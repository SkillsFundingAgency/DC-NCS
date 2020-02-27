using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models.Reports;
using ESFA.DC.NCS.ReportingService.Mappers;

namespace ESFA.DC.NCS.ReportingService.Reports
{
    public class OccupancyReport : IModelReport
    {
        private readonly ICsvFileService _csvFileService;
        private readonly IFilenameService _filenameService;
        private readonly ILogger _logger;

        public OccupancyReport(ICsvFileService csvFileService, IFilenameService filenameService, ILogger logger)
        {
            _csvFileService = csvFileService;
            _filenameService = filenameService;
            _logger = logger;
        }

        public async Task<string[]> GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Generating Occupancy Report");

            var fileName = _filenameService.GetFilename(ncsJobContextMessage.Ukprn, ncsJobContextMessage.JobId, ReportNameConstants.Occupancy, ncsJobContextMessage.DssTimeStamp, OutputTypes.Csv);

            var reportData = GetOccupancyReportModel(data, ncsJobContextMessage);

            await _csvFileService.WriteAsync<OccupancyReportModel, OccupancyReportMapper>(reportData, fileName, ncsJobContextMessage.DctContainer, cancellationToken);

            _logger.LogInfo("Occupancy Report generated");

            return new[] { fileName };
        }

        private IEnumerable<OccupancyReportModel> GetOccupancyReportModel(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage)
        {
            return data
                .Where(d => d.OutcomeEffectiveDate <= ncsJobContextMessage.ReportEndDate)
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
                    PeriodValues = BuildPeriodValues(d)
                });
        }

        private int[] BuildPeriodValues(ReportDataModel reportData)
        {
            var periodValues = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            periodValues[reportData.OutcomeEffectiveDate.Month - 1] = reportData.Value;

            return periodValues;
        }
    }
}
