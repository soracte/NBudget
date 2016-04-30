using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using NBudget.Models;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;

namespace WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private const string EndpointUri = "https://nbdocumentdb.documents.azure.com:443/";
        private const string PrimaryKey = "LXFOCBF2F6DiAof2Y4aUJ14mL5zq7YX44b3cWVMaNvGz4gsKWdpf8Fi1GN6rj0J1kEwy8lGkEVUhFFlz98Suuw==";

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private NBudgetContext db;
        private CloudQueue reportQueue;
        private DocumentClient dc;

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

            var dbConnString = CloudConfigurationManager.GetSetting("NBudgetContextConnectionString");
            db = new NBudgetContext(dbConnString);

            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            reportQueue = queueClient.GetQueueReference("images");
            reportQueue.CreateIfNotExists();

            dc = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
            CreateDatabaseIfNotExists("reports").Wait();
            CreateDocumentCollectionIfNotExists("reports", "ReportCollection").Wait();

            CreateFamilyDocumentIfNotExists("reports", "ReportCollection", new Report() { Id = "TestReport" + new Random().Next(), CreationDate = DateTime.Now }).Wait();
            //            dc.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri("reports"));

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
            var contents = JsonConvert.DeserializeObject(message.AsString);
            Console.WriteLine(contents);
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
                    await dc.CreateDatabaseAsync(new Database { Id = databaseName });
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

        private async Task CreateFamilyDocumentIfNotExists(string databaseName, string collectionName, Report report) 
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
