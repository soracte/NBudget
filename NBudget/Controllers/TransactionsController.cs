using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using NBudget.Models;
using NBudget.Controllers.ApiControllers;
using NBudget.Persistence;

namespace NBudget.Controllers
{
    [RoutePrefix("api/Transactions")]
    public class TransactionsController : BaseApiController 
    {
        private NBudgetContext db = new NBudgetContext();
        private EntityQuery<Transaction> transactions = new EntityQuery<Transaction>();
        private EntityQuery<Category> cats = new EntityQuery<Category>();

        [Route("{userId}")]
        public IHttpActionResult GetTransactionsByCategory(string userId, [FromUri] int[] cats = null)
        {
            var retval = transactions.EntitiesOfUser(db.Transactions, userId)
                .Where(t => cats.Count() == 0 || cats.Contains(t.Category.Id))
                .Select(t =>
            new
            {
                Date = t.Date,
                Amount = t.Amount,
                Reason = t.Reason,
                Category = t.Category.Id
            });

            return Ok(retval);
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        [Route("{userId}/{id}")]
        public IHttpActionResult GetTransaction(string userId, int id)
        {
            Transaction transaction = transactions.EntitiesOfUser(db.Transactions, userId).SingleOrDefault(t => t.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [ResponseType(typeof(void))]
        [Route("{userId}/{id}")]
        public IHttpActionResult PutTransaction(string userId, int id, TransactionDTO updatedDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Transaction updated = transactions.EntitiesOfUser(db.Transactions, userId).SingleOrDefault(t => t.Id == id);

            if (updated == null)
            {
                return NotFound();
            }

            db.Entry(updated).State = EntityState.Modified;
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(Transaction))]
        [Route("{userId}", Name = "PostTransaction")]
        public IHttpActionResult PostTransaction(string userId, TransactionDTO transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Category category = cats.EntitiesOfUser(db.Categories, userId).SingleOrDefault(c => c.Id == transaction.Category);

            if (category == null)
            {
                return NotFound();
            }

            ApplicationUser user = db.Users.SingleOrDefault(u => u.Id == userId) as ApplicationUser;
            if (user == null)
            {
                return NotFound();
            }

            Transaction addedTransaction = new Transaction
            {
                Date = transaction.Date,
                Amount = transaction.Amount,
                Category = category,
                Reason = transaction.Reason,
                Owner = user
            };

            db.Transactions.Add(addedTransaction);
            db.SaveChanges();

            return CreatedAtRoute("PostTransaction", new { id = addedTransaction.Id }, transaction);
        }

        [ResponseType(typeof(Transaction))]
        [Route("{userId}/{id}")]
        public IHttpActionResult DeleteTransaction(string userId, int id)
        {
            Transaction transaction = transactions.EntitiesOfUser(db.Transactions, userId).SingleOrDefault(t => t.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            db.Transactions.Remove(transaction);
            db.SaveChanges();

            return Ok(transaction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}