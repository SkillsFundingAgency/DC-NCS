using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.Service.Helpers
{
    public class ModelBuilder : IModelBuilder
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public ModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public IEnumerable<NcsSubmission> BuildNcsSubmission(IEnumerable<DssDataModel> dssData, INcsJobContextMessage ncsJobContextMessage)
        {
            return dssData.Select(item => new NcsSubmission
                {
                    ActionPlanId = item.ActionPlanId,
                    AdviserName = item.AdviserName,
                    CustomerId = item.CustomerID,
                    DateOfBirth = item.DateOfBirth,
                    HomePostCode = item.HomePostCode,
                    OutcomeEffectiveDate = item.OutcomeEffectiveDate,
                    OutcomeId = item.OutcomeId,
                    OutcomePriorityCustomer = item.OutcomePriorityCustomer,
                    OutcomeType = item.OutcomeType,
                    SubContractorId = item.SubContractorId,
                    SessionDate = item.SessionDate,
                    Ukprn = ncsJobContextMessage.Ukprn,
                    TouchpointId = ncsJobContextMessage.TouchpointId,
                    DssJobId = ncsJobContextMessage.DssJobId,
                    DssTimestamp = ncsJobContextMessage.DssTimeStamp,
                    CollectionYear = ncsJobContextMessage.CollectionYear,
                    CreatedOn = _dateTimeProvider.GetNowUtc()
                });
        }

        public ICollection<ReportDataModel> BuildReportData(IEnumerable<NcsSubmission> submissionData, IEnumerable<FundingValue> fundingValue)
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

        public Source BuildSourceData(INcsJobContextMessage ncsJobContextMessage)
        {
            return new Source()
            {
                Ukprn = ncsJobContextMessage.Ukprn,
                TouchpointId = ncsJobContextMessage.TouchpointId,
                SubmissionDate = ncsJobContextMessage.DssTimeStamp,
                DssJobId = ncsJobContextMessage.DssJobId,
                CreatedOn = _dateTimeProvider.GetNowUtc()
            };
        }
    }
}
