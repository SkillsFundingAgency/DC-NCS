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
using ESFA.DC.NCS.ReportingService.Constants;

namespace ESFA.DC.NCS.ReportingService.Reports
{
    public class OccupancyReport : IModelReport
    {
        private readonly ICsvFileService _csvFileService;
        private readonly IFilenameService _filenameService;
        private readonly IClassMapFactory<OccupancyReportModel> _classMapFactory;
        private readonly ILogger _logger;

        private readonly IDictionary<int, string> _outcomeTypesDictionary = new Dictionary<int, string>()
        {
            { 1, OutcomeConstants.CustomerSatisfaction },
            { 2, OutcomeConstants.CareerManagement },
            { 3, OutcomeConstants.SustainableEmployment },
            { 4, OutcomeConstants.AccreditedLearning },
            { 5, OutcomeConstants.CareerProgression },
        };

        public OccupancyReport(
            ICsvFileService csvFileService,
            IFilenameService filenameService,
            IClassMapFactory<OccupancyReportModel> classMapFactory,
            ILogger logger)
        {
            _csvFileService = csvFileService;
            _filenameService = filenameService;
            _classMapFactory = classMapFactory;
            _logger = logger;
        }

        public async Task<string[]> GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Generating Occupancy Report");

            var fileName = _filenameService.GetFilename(ncsJobContextMessage.Ukprn, ncsJobContextMessage.JobId, ReportNameConstants.Occupancy, ncsJobContextMessage.DssTimeStamp, OutputTypes.Csv);

            var reportData = GetOccupancyReportModel(data, ncsJobContextMessage);

            var mapper = _classMapFactory.Build(ncsJobContextMessage);

            await _csvFileService.WriteAsync(reportData, fileName, ncsJobContextMessage.DctContainer, cancellationToken, classMap: mapper);

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
                    OutcomeType = _outcomeTypesDictionary[d.OutcomeType],
                    OutcomeEffectiveDate = d.OutcomeEffectiveDate,
                    OutcomePriorityCustomer = d.OutcomePriorityCustomer,
                    PeriodValues = BuildPeriodValues(d)
                });
        }

        private int?[] BuildPeriodValues(ReportDataModel reportData)
        {
            var periodValues = new int?[] { null, null, null, null, null, null, null, null, null, null, null, null };

            periodValues[reportData.OutcomeEffectiveDate.Month - 1] = reportData.Value;

            return periodValues;
        }
    }
}
