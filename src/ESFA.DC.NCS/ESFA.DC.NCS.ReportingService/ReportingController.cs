using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.FileService.Interface;
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
        private readonly IFileService _dctFileService;
        private readonly IFileService _dssFileService;

        public ReportingController(
            IEnumerable<IModelReport> ncsReports,
            IZipService zipService,
            IFilenameService filenameService,
            [KeyFilter(PersistenceStorageKeys.DctAzureStorage)] IFileService dctFileService,
            [KeyFilter(PersistenceStorageKeys.DssAzureStorage)] IFileService dssFileService)
        {
            _ncsReports = ncsReports;
            _zipService = zipService;
            _filenameService = filenameService;
            _dctFileService = dctFileService;
            _dssFileService = dssFileService;
        }

        public async Task ProduceReportsAsync(IEnumerable<ReportDataModel> reportData, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            var reportOutputFileNames = new List<string>();

            foreach (var report in _ncsReports)
            {
                var reportsGenerated = await report.GenerateReport(reportData, ncsJobContextMessage, cancellationToken);
                reportOutputFileNames.AddRange(reportsGenerated);
            }

            var zipName = _filenameService.GetZipName(ncsJobContextMessage.Ukprn, ncsJobContextMessage.JobId, "Reports");

            await _zipService.CreateZipAsync(zipName, reportOutputFileNames, ncsJobContextMessage.DctContainer, cancellationToken);

            await CopyZipToDss(zipName, ncsJobContextMessage, cancellationToken);
        }

        private async Task CopyZipToDss(string dctZipName, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (var fileStream = await _dssFileService.OpenWriteStreamAsync($"{ncsJobContextMessage.ReportFileName}", ncsJobContextMessage.DssContainer, cancellationToken))
            {
                using (var readStream = await _dctFileService.OpenReadStreamAsync(dctZipName, ncsJobContextMessage.DctContainer, cancellationToken))
                {
                    await readStream.CopyToAsync(fileStream, 8096, cancellationToken);
                }
            }
        }
    }
}
