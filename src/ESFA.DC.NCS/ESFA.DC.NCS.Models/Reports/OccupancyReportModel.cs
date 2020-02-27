using System;

namespace ESFA.DC.NCS.Models.Reports
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
        public int[] PeriodValues { get; set; }
        public int April => PeriodValues[3];
        public int May => PeriodValues[4];
        public int June => PeriodValues[5];
        public int July => PeriodValues[6];
        public int August => PeriodValues[7];
        public int September => PeriodValues[8];
        public int October => PeriodValues[9];
        public int November => PeriodValues[10];
        public int December => PeriodValues[11];
        public int January => PeriodValues[0];
        public int February => PeriodValues[1];
        public int March => PeriodValues[2];
        public string OfficialSensitive { get; }
    }
}
