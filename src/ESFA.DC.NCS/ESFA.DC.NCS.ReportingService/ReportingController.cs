using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.ReportingService
{
    public class ReportingController : IReportingController
    {
        private readonly IEnumerable<IModelReport> _ncsReports;
        private readonly IZipService _zipService;
        private readonly IFilenameService _filenameService;
        private readonly IStreamableKeyValuePersistenceService _dctStorage;
        private readonly IStreamableKeyValuePersistenceService _ncsStorage;

        public ReportingController(
            IEnumerable<IModelReport> ncsReports,
            IZipService zipService,
            IFilenameService filenameService,
            [KeyFilter(PersistenceStorageKeys.DctAzureStorage)] IStreamableKeyValuePersistenceService dctStorage,
            [KeyFilter(PersistenceStorageKeys.NcsAzureStorage)] IStreamableKeyValuePersistenceService ncsStorage)
        {
            _ncsReports = ncsReports;
            _zipService = zipService;
            _filenameService = filenameService;
            _dctStorage = dctStorage;
            _ncsStorage = ncsStorage;
        }

        public async Task ProduceReportsAsync(IEnumerable<ReportDataModel> reportData, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            var reportOutputFileNames = new List<string>();

            foreach (var report in _ncsReports)
            {
                var reportsGenerated = await report.GenerateReport(reportData, ncsJobContextMessage, cancellationToken);
                reportOutputFileNames.AddRange(reportsGenerated);
            }

            var zipName = _filenameService.GetZipName(ncsJobContextMessage.Ukprn, ncsJobContextMessage.JobId, ncsJobContextMessage.ReportFileName);

            await _zipService.CreateZipAsync(zipName, reportOutputFileNames, ncsJobContextMessage.DctContainer, cancellationToken);
        }
    }
}
