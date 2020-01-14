using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.Interfaces.ReportingService
{
    public interface IModelReport
    {
        Task<string[]> GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
