using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.NCS.Interfaces.ReportingService
{
    public class OccupancyReportModel
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
        public string OfficialSensitive { get; }
    }
}
