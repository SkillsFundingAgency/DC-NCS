using CsvHelper.Configuration;
using ESFA.DC.NCS.Interfaces.ReportingService;

namespace ESFA.DC.NCS.ReportingService.Mappers
{
    public sealed class OccupancyReportMapper : ClassMap<OccupancyReportModel>
    {
        public OccupancyReportMapper()
        {
            int i = 0;
            Map(m => m.CustomerId).Index(i++).Name("Customer ID");
            Map(m => m.DateOfBirth).Index(i++).Name("Date of Birth").TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.HomePostCode).Index(i++).Name("Home Postcode");
            Map(m => m.ActionPlanId).Index(i++).Name("Action Plan ID");
            Map(m => m.SessionDate).Index(i++).Name("Session Date").TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.SubContractorId).Index(i++).Name("Subcontractor ID");
            Map(m => m.AdviserName).Index(i++).Name("Adviser Name");
            Map(m => m.OutcomeId).Index(i++).Name("Outcome ID");
            Map(m => m.OutcomeType).Index(i++).Name("Outcome Type");
            Map(m => m.OutcomeEffectiveDate).Index(i++).Name("Outcome Effective Date").TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.OutcomePriorityCustomer).Index(i++).Name("Outcome Priority Group");
            Map(m => m.Period).Index(i++).Name("Period");
            Map(m => m.Value).Index(i++).Name("Value");
            Map(m => m.OfficialSensitive).Index(i).Name("OFFICIAL-SENSITIVE");
        }
    }
}
