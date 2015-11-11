using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBudget.Models
{
    public class TransactionDTO
    {
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; }
        public int Category { get; set; }
    }
}