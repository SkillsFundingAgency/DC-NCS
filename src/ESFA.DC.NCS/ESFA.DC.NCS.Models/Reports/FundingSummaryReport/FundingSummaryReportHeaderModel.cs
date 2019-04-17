using System;

namespace ESFA.DC.NCS.Models.Reports.FundingSummaryReport
{
    public class FundingSummaryReportHeaderModel
    {
        public string ProviderName { get; set; }

        public string TouchpointId { get; set; }

        public DateTime LastNcsUpdate { get; set; }

        public string SecurityClassification => "OFFICIAL-SENSITIVE";
    }
}
