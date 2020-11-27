using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.Interfaces.ReportingService
{
    public interface IReportingController
    {
        Task ProduceReportsAsync(IEnumerable<ReportDataModel> reportData, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
