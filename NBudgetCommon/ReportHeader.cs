using System;

namespace NBudget.Models
{
    public class ReportHeader : OwnedEntity 
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ReportDocumentId { get; set; }

   }
}
