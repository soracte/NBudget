using Microsoft.AspNet.Identity;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NBudget.Attributes;
using NBudget.Controllers.ApiControllers;
using NBudget.Models;
using NBudget.Persistence;
using NBudgetCommon;
using NBudgetCommon.Factory;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Http;

namespace NBudget.Controllers
{
    [RoutePrefix("api/ReportHeaders")]
    public class ReportHeadersController : BaseApiController
    {
        private EntityQuery<ReportHeader> reportHeaders = new EntityQuery<ReportHeader>();

        // GET: api/ReportHeaders
        [Route("{userId}")]
        [AnotherUserAuthorize]
        public IHttpActionResult GetReportHeaders(string userId)
        {
            var retval = reportHeaders.EntitiesOfUser(db.ReportHeaders, userId)
                .Select(r => new
            {
                Id = r.Id,
                Created = r.CreationDate,
                ReportDocumentId = r.ReportDocumentId,
                FromDate = r.FromDate,
                ToDate = r.ToDate
            });

            return Ok(retval);
        }

        // GET: api/ReportHeaders
        [Route("{userId}/{reportHeaderId}")]
        [AnotherUserAuthorize]
        public IHttpActionResult GetReportHeader(string userId, int reportHeaderId)
        {
            ReportHeader header = reportHeaders.EntitiesOfUser(db.ReportHeaders, userId).SingleOrDefault(rh => rh.Id == reportHeaderId);

            if (header == null)
            {
                return NotFound();
            }

            DocumentClient dc = DocumentClientFactory.CreateDocumentClient();
            IQueryable<Report> reportDocuments = dc.CreateDocumentQuery<Report>(UriFactory.CreateDocumentCollectionUri("reports", "ReportCollection"))
                .Where(r => r.Id == header.ReportDocumentId);

            Report report = null;
            foreach (Report reportItem in reportDocuments)
            {
                report = reportItem;
            }

            if (report == null)
            {
                return NotFound();
            }

            var retval = new
            {
                Id = header.Id,
                Created = header.CreationDate,
                ReportDocumentId = header.ReportDocumentId,
                FromDate = header.FromDate,
                ToDate = header.ToDate,
                TopTransactions = report.TopTransactions,
                CategorySummary = report.CategorySummaries
            };

            return Ok(retval);
        }

        // POST: api/ReportHeaders
        [Route("{userId}")]
        [AnotherUserAuthorize]
        public IHttpActionResult PostReportHeader(string userId, [FromBody] NewReportBindingModel model)
        {
            ApplicationUser currentUser = UserManager.FindById(userId);

            ReportHeader header = new ReportHeader()
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                CreationDate = DateTime.Now,
                Owner = currentUser
            };

            db.ReportHeaders.Add(header);
            db.SaveChanges();

            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue reportQueue = queueClient.GetQueueReference("reports");

            CreateReportMessageContent content = new CreateReportMessageContent()
            {
                ReportId = header.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                OwnerId = currentUser.Id 
            };

            CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(content));

            reportQueue.AddMessage(message);

            return Ok(new { Message = "Report requested." });

        }

        public class NewReportBindingModel
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }
    }
}
