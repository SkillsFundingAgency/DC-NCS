using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.NCS.Interfaces.Service;

namespace ESFA.DC.NCS.Service.Services
{
    public class FundingService : IFundingService
    {
        private readonly IOutcomeRateQueryService _outcomeRateService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public FundingService(IOutcomeRateQueryService outcomeRateService, IDateTimeProvider dateTimeProvider, ILogger logger)
        {
            _outcomeRateService = outcomeRateService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<FundingValue>> CalculateFunding(IEnumerable<NcsSubmission> ncsSubmissions, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            var priorityCommunityRate = _outcomeRateService.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.Priority, OutcomeRatesConstants.Community, ncsJobContextMessage.DssTimeStamp, cancellationToken).Result;
            var nonPriorityCommunityRate = _outcomeRateService.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.NonPriority, OutcomeRatesConstants.Community, ncsJobContextMessage.DssTimeStamp, cancellationToken).Result;

            var fundingValues = new List<FundingValue>();

            foreach (var submission in ncsSubmissions)
            {
                FundingValue fundingValue;

                switch (submission.OutcomePriorityCustomer)
                {
                    case OutcomeRatesConstants.Priority:
                        fundingValue = BuildFundingValue(submission, priorityCommunityRate);
                        break;
                    case OutcomeRatesConstants.NonPriority:
                        fundingValue = BuildFundingValue(submission, nonPriorityCommunityRate);
                        break;
                    default:
                        throw new Exception($"The outcome priority customer {submission.OutcomePriorityCustomer}, is not a valid value");
                }

                fundingValues.Add(fundingValue);
            }

            return fundingValues;
        }

        private FundingValue BuildFundingValue(NcsSubmission submission, OutcomeRate outcomeRate)
        {
            return new FundingValue()
            {
                Ukprn = submission.Ukprn,
                TouchpointId = submission.TouchpointId,
                CustomerId = submission.CustomerId,
                ActionPlanId = submission.ActionPlanId,
                OutcomeId = submission.OutcomeId,
                OutcomeType = submission.OutcomeType,
                OutcomeEffectiveDate = submission.OutcomeEffectiveDate,
                OutcomePriorityCustomer = submission.OutcomePriorityCustomer,
                Value = CalculateValue(submission.OutcomeType, outcomeRate),
                Period = submission.OutcomeEffectiveDate.ToString("MMMM"),
                CreatedOn = _dateTimeProvider.GetNowUtc()
            };
        }

        private int CalculateValue(int outcomeType, OutcomeRate outcomeRate)
        {
            if (outcomeType == OutcomeTypesConstants.CustomerSatisfaction)
            {
                return outcomeRate.CustomerSatisfaction;
            }

            if (outcomeType == OutcomeTypesConstants.CareerManagement)
            {
                return outcomeRate.CareerManagement;
            }

            if (OutcomeTypesConstants.JobsAndLearning.Contains(outcomeType))
            {
                return outcomeRate.JobsAndLearning;
            }

            throw new Exception($"The outcome type {outcomeType}, doesn't correspond with an outcome rate");
        }
    }
}
