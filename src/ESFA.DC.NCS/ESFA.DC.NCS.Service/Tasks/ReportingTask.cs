using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Interfaces.Service;

namespace ESFA.DC.NCS.Service.Tasks
{
    public class ReportingTask : INcsDataTask
    {
        private readonly ILogger _logger;
        private readonly IModelBuilder _modelBuilder;
        private readonly IFundingValueQueryService _fundingValueQueryService;
        private readonly INcsSubmissionQueryService _ncsSubmissionQueryService;
        private readonly IReportingController _reportingController;
        private readonly IPersistenceService _persistenceService;

        public ReportingTask(
            ILogger logger,
            IModelBuilder modelBuilder,
            IFundingValueQueryService fundingValueQueryService,
            INcsSubmissionQueryService ncsSubmissionQueryService,
            IReportingController reportingController,
            IPersistenceService persistenceService)
        {
            _logger = logger;
            _modelBuilder = modelBuilder;
            _fundingValueQueryService = fundingValueQueryService;
            _ncsSubmissionQueryService = ncsSubmissionQueryService;
            _reportingController = reportingController;
            _persistenceService = persistenceService;
        }

        public string TaskName => TaskNameConstants.ReportingTaskName;

        public async Task ExecuteAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Debug.WriteLine("Entered Reporting Task");

            try
            {
                var submissionData = await _ncsSubmissionQueryService.GetNcsSubmissionsAsync(ncsJobContextMessage, cancellationToken);
                var fundingData = await _fundingValueQueryService.GetFundingValuesAsync(ncsJobContextMessage, cancellationToken);

                var reportData = _modelBuilder.BuildReportData(submissionData, fundingData);
                await _reportingController.ProduceReportsAsync(reportData, ncsJobContextMessage, cancellationToken);

                var sourceData = _modelBuilder.BuildSourceData(ncsJobContextMessage);
                await _persistenceService.PersistSourceDataAsync(sourceData, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Reporting Task Failed", ex);
                throw;
            }
        }
    }
}
