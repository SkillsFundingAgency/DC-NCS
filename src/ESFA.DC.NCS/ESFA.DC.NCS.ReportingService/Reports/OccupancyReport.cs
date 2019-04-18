using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.IO;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Models.Reports;
using ESFA.DC.NCS.ReportingService.Mappers;

namespace ESFA.DC.NCS.ReportingService.Reports
{
    public class OccupancyReport : AbstractReportBuilder, IModelReport
    {
        private readonly IStreamProviderService _streamProviderService;
        private readonly ILogger _logger;

        public OccupancyReport(IStreamProviderService streamProviderService, ILogger logger)
        {
            _streamProviderService = streamProviderService;
            _logger = logger;
            ReportFileName = "NCS Occupancy Report";
        }

        public async Task GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInfo("Generating Occupancy Report");

            var fileName = GetFilename(ncsJobContextMessage.DssTimeStamp);

            var reportData = GetOccupancyReportModel(data, ncsJobContextMessage.DssTimeStamp);

            using (var stream = _streamProviderService.GetStream(archive, $"{fileName}.csv"))
            {
                CreateCsv(reportData, stream, cancellationToken);
            }

            _logger.LogInfo("Occupancy Report generated");
        }

        private void CreateCsv(IEnumerable<OccupancyReportModel> reportData, Stream stream, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var textWriter = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(textWriter))
            {
                WriteCsvRecords<OccupancyReportMapper, OccupancyReportModel>(csvWriter, reportData);
            }
        }

        private IEnumerable<OccupancyReportModel> GetOccupancyReportModel(IEnumerable<ReportDataModel> data, DateTime submissionDate)
        {
            // TODO: Need clarification on the date to filter on - waiting for collection dates
            return data
                    .Where(d => d.OutcomeEffectiveDate <= submissionDate)
                    .OrderBy(d => d.CustomerId)
                    .ThenBy(d => d.ActionPlanId)
                    .ThenBy(d => d.OutcomeId)
                    .Select(d => new OccupancyReportModel()
                    {
                        CustomerId = d.CustomerId,
                        DateOfBirth = d.DateOfBirth,
                        HomePostCode = d.HomePostCode,
                        ActionPlanId = d.ActionPlanId,
                        SessionDate = d.SessionDate,
                        SubContractorId = d.SubContractorId,
                        AdviserName = d.AdviserName,
                        OutcomeId = d.OutcomeId,
                        OutcomeType = d.OutcomeType,
                        OutcomeEffectiveDate = d.OutcomeEffectiveDate,
                        OutcomePriorityCustomer = d.OutcomePriorityCustomer,
                        Period = d.Period,
                        Value = d.Value
                    });
        }
    }
}
