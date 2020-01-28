using System;

namespace ESFA.DC.NCS.Models.Reports
{
    public class ReportDataModel
    {
        public Guid CustomerId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string HomePostCode { get; set; }
        public Guid ActionPlanId { get; set; }
        public DateTime SessionDate { get; set; }
        public string SubContractorId { get; set; }
        public string AdviserName { get; set; }
        public Guid OutcomeId { get; set; }
        public int OutcomeType { get; set; }
        public DateTime OutcomeEffectiveDate { get; set; }
        public int OutcomePriorityCustomer { get; set; }
        public int Value { get; set; }
        public string Period { get; set; }
    }
}
