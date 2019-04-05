using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.Interfaces.ReportingService
{
    public interface IModelReport
    {
        Task GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, ZipArchive archive, CancellationToken cancellationToken);
    }
}
