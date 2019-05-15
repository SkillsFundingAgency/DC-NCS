using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.ReportingService
{
    public class ReportingController : IReportingController
    {
        private readonly IEnumerable<IModelReport> _ncsReports;
        private readonly IStreamableKeyValuePersistenceService _dctStorage;
        private readonly IStreamableKeyValuePersistenceService _ncsStorage;

        public ReportingController(
            IEnumerable<IModelReport> ncsReports,
            [KeyFilter(PersistenceStorageKeys.DctAzureStorage)] IStreamableKeyValuePersistenceService dctStorage,
            [KeyFilter(PersistenceStorageKeys.NcsAzureStorage)] IStreamableKeyValuePersistenceService ncsStorage)
        {
            _ncsReports = ncsReports;
            _dctStorage = dctStorage;
            _ncsStorage = ncsStorage;
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

                await _dctStorage.SaveAsync($"{ncsJobContextMessage.Ukprn}_{ncsJobContextMessage.JobId}_{FileNameConstants.Reports}.zip", memoryStream, cancellationToken);
                await _ncsStorage.SaveAsync($"{ncsJobContextMessage.ReportFileName}.zip", memoryStream, cancellationToken);
            }
        }
    }
}
