using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.Service.Tasks
{
    public class ReportingTask : INcsDataTask
    {
        private readonly ILogger _logger;
        private readonly IFundingValueQueryService _fundingValueQueryService;
        private readonly INcsSubmissionQueryService _ncsSubmissionQueryService;
        private readonly IReportingController _reportingController;
        private readonly IPersistenceService _persistenceService;

        public ReportingTask(
            ILogger logger,
            IFundingValueQueryService fundingValueQueryService,
            INcsSubmissionQueryService ncsSubmissionQueryService,
            IReportingController reportingController,
            IPersistenceService persistenceService)
        {
            _logger = logger;
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
                var reportData = BuildReportData(submissionData, fundingData);

                await _reportingController.ProduceReportsAsync(reportData, ncsJobContextMessage, cancellationToken);
                await _persistenceService.PersistSourceDataAsync(ncsJobContextMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Reporting Task Failed", ex);
                throw;
            }
        }

        private ICollection<ReportDataModel> BuildReportData(IEnumerable<NcsSubmission> submissionData, IEnumerable<FundingValue> fundingValue)
        {
            return submissionData
                .Join(
                    fundingValue,
                    sd => new { sd.TouchpointId, sd.ActionPlanId, sd.CustomerId, sd.OutcomeId },
                    fv => new { fv.TouchpointId, fv.ActionPlanId, fv.CustomerId, fv.OutcomeId },
                    (sd, fv) => new ReportDataModel
                    {
                        CustomerId = sd.CustomerId,
                        DateOfBirth = sd.DateOfBirth,
                        HomePostCode = sd.HomePostCode,
                        ActionPlanId = sd.ActionPlanId,
                        SessionDate = sd.SessionDate,
                        SubContractorId = sd.SubContractorId,
                        AdviserName = sd.AdviserName,
                        OutcomeId = sd.OutcomeId,
                        OutcomeType = sd.OutcomeType,
                        OutcomeEffectiveDate = sd.OutcomeEffectiveDate,
                        OutcomePriorityCustomer = sd.OutcomePriorityCustomer,
                        Value = fv.Value,
                        Period = fv.Period
                    }).ToList();
        }
    }
}
