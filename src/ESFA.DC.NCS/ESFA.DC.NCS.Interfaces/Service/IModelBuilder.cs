using System.Collections.Generic;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Models;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IModelBuilder
    {
        IEnumerable<NcsSubmission> BuildNcsSubmission(IEnumerable<DssDataModel> dssData, INcsJobContextMessage ncsJobContextMessage);

        ICollection<ReportDataModel> BuildReportData(IEnumerable<NcsSubmission> submissionData, IEnumerable<FundingValue> fundingValue);

        Source BuildSourceData(INcsJobContextMessage ncsJobContextMessage);
    }
}
