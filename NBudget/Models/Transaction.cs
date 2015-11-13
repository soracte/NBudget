using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NBudget.Models
{
    public class Transaction
    {        
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]       
        public int Amount { get; set; }

        [Required]
        public string Reason { get; set; }
        
        [Required]
        public virtual Category Category { get; set; }

    }
}