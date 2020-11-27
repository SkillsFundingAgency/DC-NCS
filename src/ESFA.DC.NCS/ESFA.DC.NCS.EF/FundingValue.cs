using System;
using System.Collections.Generic;

namespace ESFA.DC.NCS.EF
{
    public partial class FundingValue
    {
        public int UKPRN { get; set; }
        public string TouchpointId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ActionPlanId { get; set; }
        public Guid OutcomeId { get; set; }
        public int OutcomeType { get; set; }
        public DateTime OutcomeEffectiveDate { get; set; }
        public int OutcomePriorityCustomer { get; set; }
        public int Value { get; set; }
        public string Period { get; set; }
        public int OutcomeEffectiveDateMonth { get; set; }
        public int CollectionYear { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
