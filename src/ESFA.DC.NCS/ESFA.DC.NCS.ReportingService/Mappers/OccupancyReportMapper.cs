using CsvHelper.Configuration;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Models.Reports;

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
            Map(m => m.April).Index(i++).Name("April 2019 £");
            Map(m => m.May).Index(i++).Name("May 2019 £");
            Map(m => m.June).Index(i++).Name("June 2019 £");
            Map(m => m.July).Index(i++).Name("July 2019 £");
            Map(m => m.August).Index(i++).Name("August 2019 £");
            Map(m => m.September).Index(i++).Name("September 2019 £");
            Map(m => m.October).Index(i++).Name("October 2019 £");
            Map(m => m.November).Index(i++).Name("November 2019 £");
            Map(m => m.December).Index(i++).Name("December 2019 £");
            Map(m => m.January).Index(i++).Name("January 2020 £");
            Map(m => m.February).Index(i++).Name("February 2020 £");
            Map(m => m.March).Index(i++).Name("March 2020 £");
            Map(m => m.OfficialSensitive).Index(i).Name("OFFICIAL-SENSITIVE");
        }
    }
}
