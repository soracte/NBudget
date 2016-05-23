using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using NBudget.Models;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using NBudgetCommon;
using System.Data.Entity;
using System.Linq;
using NBudget.Persistence;
using NBudgetCommon.Factory;
using Microsoft.Azure;

namespace WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private NBudgetContext db;
        private CloudQueue reportQueue;
        private DocumentClient dc;

        private EntityQuery<Transaction> transactionsQuery = new EntityQuery<Transaction>();

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole is running");

            try
            {
                RunAsync(cancellationTokenSource.Token).Wait();
            }
            finally
            {
                runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            db = NBudgetContext.Create(); 

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=nbudgetstorage;AccountKey=j6JUpfDbdh099iShN1xw+x/FEejEFAoeh0YuXI7pvDO/emoOqlQ0Y0hzXP7fxpIkYNUoFl28r/Dyd7viWIKDHg==");

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            reportQueue = queueClient.GetQueueReference("reports");
            reportQueue.CreateIfNotExists();

            dc = DocumentClientFactory.CreateDocumentClient();
            CreateDatabaseIfNotExists("reports").Wait();
            CreateDocumentCollectionIfNotExists("reports", "ReportCollection").Wait();

            //CreateReportIfNotExists("reports", "ReportCollection", new Report() { Id = "TestReport" + new Random().Next(), CreationDate = DateTime.Now }).Wait();
                        //dc.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri("reports"));

            Trace.TraceInformation("WorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole is stopping");

            cancellationTokenSource.Cancel();
            runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            CloudQueueMessage msg = null;

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                msg = reportQueue.GetMessage();

                if (msg != null)
                {
                    ProcessMessage(msg);
                }
                else
                {
                    Trace.TraceInformation("Working");
                    await Task.Delay(1000);
                }
            }
        }

        private void ProcessMessage(CloudQueueMessage message)
        {
            CreateReportMessageContent content = JsonConvert.DeserializeObject<CreateReportMessageContent>(message.AsString);


            CategorySummary[] categorySummary = CreateCategorySummary(content);
            TransactionInfo[] topTransactions = CreateTopTransactions(content);

            Report report = new Report()
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = content.OwnerId,
                FromDate = content.FromDate,
                ToDate = content.ToDate,
                CreationDate = DateTime.Now,
                CategorySummaries = categorySummary,
                TopTransactions = topTransactions 
            };

            CreateReportIfNotExists("reports", "ReportCollection", report).Wait();

            ReportHeader header = db.ReportHeaders.Find(content.ReportId);
            db.Entry(header).Reference(r => r.Owner).Load();
            header.ReportDocumentId = report.Id;
            db.SaveChanges();

            reportQueue.DeleteMessage(message);
        }

        private TransactionInfo[] CreateTopTransactions(CreateReportMessageContent content)
        {
            var top = transactionsQuery.EntitiesOfUser(db.Transactions, content.OwnerId)
                .Where(t =>
                t.Date >= content.FromDate &&
                t.Date <= content.ToDate)
                .OrderByDescending(t => Math.Abs(t.Amount))
                .Take(5);

            return top.Select(t => new TransactionInfo()
            {
                Amount = t.Amount,
                CategoryName = t.Category.Name,
                Reason = t.Reason
            })
            .OrderByDescending(t => Math.Abs(t.Amount))
            .ToArray();
        }

        private CategorySummary[] CreateCategorySummary(CreateReportMessageContent content)
        {
            return transactionsQuery.EntitiesOfUser(db.Transactions, content.OwnerId)
                .Where(t =>
                t.Date >= content.FromDate &&
                t.Date <= content.ToDate)
                .GroupBy(t => t.Category)
                .Select(g => new CategorySummary()
                {
                    CategoryName = g.FirstOrDefault().Category.Name,
                    Sum = g.Sum(t => t.Amount)
                }).ToArray();
        }

        private async Task CreateDatabaseIfNotExists(string databaseName)
        {
            // Check to verify a database with the id=FamilyDB does not exist
            try
            {
                await dc.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException de)
            {
                // If the database does not exist, create a new database
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await dc.CreateDatabaseAsync(new Microsoft.Azure.Documents.Database { Id = databaseName });
                }
                else
                {
                    throw;
                }
            }
        }

        // ADD THIS PART TO YOUR CODE
        private async Task CreateDocumentCollectionIfNotExists(string databaseName, string collectionName)
        {
            try
            {
                await dc.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
            catch (DocumentClientException de)
            {
                // If the document collection does not exist, create a new collection
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = collectionName;

                    // Configure collections for maximum query flexibility including string range queries.
                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                    // Here we create a collection with 400 RU/s.
                    await dc.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseName),
                        new DocumentCollection { Id = collectionName },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateReportIfNotExists(string databaseName, string collectionName, Report report) 
        {
            try
            {
                await dc.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, report.Id));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await dc.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), report);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
