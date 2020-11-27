using System;
using System.Collections.Generic;

namespace ESFA.DC.NCS.EF
{
    public partial class OutcomeRate
    {
        public int Id { get; set; }
        public int OutcomePriorityCustomer { get; set; }
        public int CustomerSatisfaction { get; set; }
        public int CareerManagement { get; set; }
        public int Jobs { get; set; }
        public int Learning { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
