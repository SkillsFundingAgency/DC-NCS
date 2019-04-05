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

        public ReportingTask(ILogger logger, IFundingValueQueryService fundingValueQueryService, INcsSubmissionQueryService ncsSubmissionQueryService, IReportingController reportingController)
        {
            _logger = logger;
            _fundingValueQueryService = fundingValueQueryService;
            _ncsSubmissionQueryService = ncsSubmissionQueryService;
            _reportingController = reportingController;
        }

        public string TaskName => TaskNameConstants.ReportingTaskName;

        public async Task ExecuteAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Debug.WriteLine("Entered Reporting Task");

            try
            {
                _logger.LogInfo("Reporting Task Started");

                var submissionData = await _ncsSubmissionQueryService.GetNcsSubmissionsAsync(ncsJobContextMessage, cancellationToken);
                var fundingData = await _fundingValueQueryService.GetFundingValuesAsync(ncsJobContextMessage, cancellationToken);
                var reportData = BuildReportData(submissionData, fundingData);

                await _reportingController.ProduceReportsAsync(reportData, ncsJobContextMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Reporting Task Failed", ex);
                throw;
            }
        }

        private IEnumerable<ReportDataModel> BuildReportData(IEnumerable<NcsSubmission> submissionData, IEnumerable<FundingValue> fundingValue)
        {
            var reportData = submissionData
                .Join(
                    fundingValue,
                    sd => new { sd.TouchpointId, sd.ActionPlanId, sd.CustomerId, sd.OutcomeId },
                    fv => new { fv.TouchpointId, fv.ActionPlanId, fv.CustomerId, fv.OutcomeId },
                    (sd, fv) => new ReportDataModel
                    {
                        Ukprn = sd.Ukprn,
                        TouchpointId = sd.TouchpointId,
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
                        CollectionYear = sd.CollectionYear,
                        DssJobId = sd.DssJobId,
                        DssTimestamp = sd.DssTimestamp,
                        CreatedOn = sd.CreatedOn,
                        Value = fv.Value,
                        Period = fv.Period
                    });

            return reportData;
        }
    }
}
