using System;
using System.Collections.Generic;

namespace ESFA.DC.NCS.EF
{
    public partial class FundingValue
    {
        public int Ukprn { get; set; }
        public string TouchpointId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ActionPlanId { get; set; }
        public Guid OutcomeId { get; set; }
        public int OutcomeType { get; set; }
        public DateTime OutcomeEffectiveDate { get; set; }
        public int OutcomePriorityGroup { get; set; }
        public int Value { get; set; }
        public string Period { get; set; }
    }
}
