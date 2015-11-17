﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using NBudget.Models;
using System.Web.Http.Cors;

namespace NBudget.Controllers
{
    [EnableCors(origins: "http://localhost:56015", headers: "*", methods: "*")]
    public class TransactionsController : ApiController
    {
        private NBudgetContext db = new NBudgetContext();

        // GET: api/Transactions
        public IHttpActionResult GetTransactions()
        {
            return Ok(db.Transactions.Select(t => 
            new
            {
                Date = t.Date,
                Amount = t.Amount,
                Reason = t.Reason,
                Category = t.Category.Id
            }));
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public IHttpActionResult GetTransaction(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTransaction(int id, Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.Id)
            {
                return BadRequest();
            }

            db.Entry(transaction).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Transactions
        [ResponseType(typeof(Transaction))]
        public IHttpActionResult PostTransaction(TransactionDTO transaction)
        {
            Category category = db.Categories.Single(c => c.Id == transaction.Category);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Transaction addedTransaction = new Transaction
            {
                Date = transaction.Date,
                Amount = transaction.Amount,
                Category = category,
                Reason = transaction.Reason
            };

            db.Transactions.Add(addedTransaction);
            db.SaveChanges();
            
            return CreatedAtRoute("DefaultApi", new { id = addedTransaction.Id }, transaction);
        }

        // DELETE: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public IHttpActionResult DeleteTransaction(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
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

        private bool TransactionExists(int id)
        {
            return db.Transactions.Count(e => e.Id == id) > 0;
        }
    }
}