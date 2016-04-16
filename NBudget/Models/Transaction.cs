using System;
using System.ComponentModel.DataAnnotations;

namespace NBudget.Models
{
    public class Transaction : OwnedEntity
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