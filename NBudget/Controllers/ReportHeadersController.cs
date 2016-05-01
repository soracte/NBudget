using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NBudget.Controllers.ApiControllers;
using NBudget.Models;
using NBudgetCommon;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Http;

namespace NBudget.Controllers
{
    public class ReportHeadersController : BaseApiController
    {
        // GET: api/ReportHeaders
        public IHttpActionResult GetReportHeaders()
        {
            var retval = db.ReportHeaders.Select(r => new
            {
                Id = r.Id,
                Created = r.CreationDate,
                ReportDocumentId = r.ReportDocumentId
            });

            return Ok(retval);
        }

        // GET: api/ReportHeaders/5
        public IHttpActionResult GetReportHeader(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            var report = db.ReportHeaders.Where(r => r.Owner.Id == currentUserId);
            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }

        // POST: api/ReportHeaders
        public IHttpActionResult PostReportHeader(NewReportBindingModel model)
        {
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());

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

            return Ok();

        }

        // PUT: api/ReportHeaders/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ReportHeaders/5
        public void Delete(int id)
        {
        }

        public class NewReportBindingModel
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }
    }
}
