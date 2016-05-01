using System;

namespace NBudgetCommon
{
    public class CreateReportMessageContent
        {
            public int ReportId { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string OwnerId { get; set; }
    }
}
