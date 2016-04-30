using Newtonsoft.Json;
using System;

namespace NBudget.Models 
{
    public class Report
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        //public DateTime FromDate { get; set; }
        //public DateTime ToDate { get; set; }
        //public Transaction[] TopTransactions { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
