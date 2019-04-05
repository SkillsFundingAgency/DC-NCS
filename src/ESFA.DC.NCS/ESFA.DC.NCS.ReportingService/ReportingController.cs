using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.ReportingService
{
    public class ReportingController : IReportingController
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IModelReport> _ncsReports;

        public ReportingController(ILogger logger, IEnumerable<IModelReport> ncsReports)
        {
            _logger = logger;
            _ncsReports = ncsReports;
        }

        public async Task ProduceReportsAsync(IEnumerable<ReportDataModel> reportData, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    foreach (var report in _ncsReports)
                    {
                        await report.GenerateReport(reportData, ncsJobContextMessage, archive, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }

                using (var fileStream = new FileStream($"C:\\tmp\\{ncsJobContextMessage.ReportFileName}.zip", FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
