using System.Collections.Generic;
using System.Threading;
using ESFA.DC.NCS.Models.Reports;
using ESFA.DC.NCS.Models.Reports.FundingSummaryReport;

namespace ESFA.DC.NCS.Interfaces.ReportingService
{
    public interface IFundingSummaryReportBuilder
    {
        IDictionary<string, string> BuildHeaderData(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);

        FundingSummaryColumnHeaders BuildColumnHeaders(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);

        IEnumerable<FundingSummaryReportModel> BuildPriorityGroupRows(IEnumerable<ReportDataModel> data);

        IEnumerable<FundingSummaryReportModel> BuildNonPriorityGroupRows(IEnumerable<ReportDataModel> data);

        IDictionary<string, string> BuildFooterData(CancellationToken cancellationToken);
    }
}
