using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.Interfaces.ReportingService
{
    public interface IFundingSummaryReportBuilder
    {
        IEnumerable<FundingSummaryReportModel> BuildPriorityGroupRows(IEnumerable<ReportDataModel> data);

        IEnumerable<FundingSummaryReportModel> BuildNonPriorityGroupRows(IEnumerable<ReportDataModel> data);
    }
}
