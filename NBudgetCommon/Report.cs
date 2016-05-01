using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace NBudget.Models
{
    public class Report 
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreationDate { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public virtual TransactionInfo[] TopTransactions { get; set; }

        [Required]
        public virtual CategorySummary[] CategorySummaries { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class TransactionInfo
    {
        public int Amount { get; set; }
        public string Reason { get; set; }
        public string CategoryName { get; set; }
    }


    public class CategorySummary
    {
        public string CategoryName { get; set; }
        public int Sum { get; set; }
    }
}
