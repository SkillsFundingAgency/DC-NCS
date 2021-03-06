﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
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

        public FundingService(IOutcomeRateQueryService outcomeRateService, IDateTimeProvider dateTimeProvider)
        {
            _outcomeRateService = outcomeRateService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IEnumerable<FundingValue>> CalculateFundingAsync(IEnumerable<NcsSubmission> ncsSubmissions, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            var priorityCommunityRate = await _outcomeRateService.GetOutcomeRatesByPriorityAsync(OutcomeRatesConstants.Priority, cancellationToken);
            var nonPriorityCommunityRate = await _outcomeRateService.GetOutcomeRatesByPriorityAsync(OutcomeRatesConstants.NonPriority, cancellationToken);

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

        private FundingValue BuildFundingValue(NcsSubmission submission, IEnumerable<OutcomeRate> outcomeRates)
        {
            var outcomeRate = GetOutcomeRate(outcomeRates, submission.OutcomeEffectiveDate, submission.OutcomePriorityCustomer);

            return new FundingValue()
            {
                UKPRN = submission.UKPRN,
                TouchpointId = submission.TouchpointId,
                CustomerId = submission.CustomerId,
                ActionPlanId = submission.ActionPlanId,
                OutcomeId = submission.OutcomeId,
                OutcomeType = submission.OutcomeType,
                OutcomeEffectiveDate = submission.OutcomeEffectiveDate,
                OutcomePriorityCustomer = submission.OutcomePriorityCustomer,
                Value = CalculateValue(submission.OutcomeType, outcomeRate),
                Period = submission.OutcomeEffectiveDate.ToString("MMMM"),
                OutcomeEffectiveDateMonth = submission.OutcomeEffectiveDate.Month,
                CollectionYear = submission.CollectionYear,
                CreatedOn = _dateTimeProvider.GetNowUtc()
            };
        }

        private OutcomeRate GetOutcomeRate(IEnumerable<OutcomeRate> outcomeRates, DateTime outcomeEffectiveDate, int outcomePriorityCustomer)
        {
            var outcomeRate = outcomeRates
                .Where(or => outcomeEffectiveDate >= or.EffectiveFrom && outcomeEffectiveDate <= (or.EffectiveTo ?? DateTime.MaxValue))
                .ToList();

            if (!outcomeRate.Any() || outcomeRate.Count > 1)
            {
                throw new Exception($"OutcomeRates table has none or multiple rates for the values: OutcomePriorityCustomer-{outcomePriorityCustomer} and outcome effective date-{outcomeEffectiveDate}");
            }

            return outcomeRate.Single();
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

            if (outcomeType == OutcomeTypesConstants.Learning)
            {
                return outcomeRate.Learning;
            }

            if (OutcomeTypesConstants.Jobs.Contains(outcomeType))
            {
                return outcomeRate.Jobs;
            }

            throw new Exception($"The outcome type {outcomeType}, doesn't correspond with an outcome rate");
        }
    }
}
