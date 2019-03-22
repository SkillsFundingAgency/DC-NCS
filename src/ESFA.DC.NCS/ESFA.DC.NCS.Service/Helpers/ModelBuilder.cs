using System.Collections.Generic;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Models;

namespace ESFA.DC.NCS.Service.Helpers
{
    public static class ModelBuilder
    {
        public static IEnumerable<NcsSubmission> BuildNcsSubmission(IEnumerable<DssDataModel> dssData, INcsJobContextMessage ncsJobContextMessage)
        {
            var submissions = new List<NcsSubmission>();

            foreach (var item in dssData)
            {
                var ncsSubmission = new NcsSubmission
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
                    ReturnPeriod = ncsJobContextMessage.ReturnPeriod,
                    CollectionYear = ncsJobContextMessage.CollectionYear
                };

                submissions.Add(ncsSubmission);
            }

            return submissions;
        }
    }
}
