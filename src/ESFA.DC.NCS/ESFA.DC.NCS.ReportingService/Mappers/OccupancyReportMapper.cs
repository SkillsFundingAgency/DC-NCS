using CsvHelper.Configuration;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.ReportingService.Mappers
{
    public sealed class OccupancyReportMapper : ClassMap<OccupancyReportModel>
    {
        public OccupancyReportMapper(int collectionYear)
        {
            var startYear = collectionYear.ToString().Substring(0, 2);
            var endYear = collectionYear.ToString().Substring(2, 2);

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
            Map(m => m.April).Index(i++).Name($"April 20{startYear} £");
            Map(m => m.May).Index(i++).Name($"May 20{startYear} £");
            Map(m => m.June).Index(i++).Name($"June 20{startYear} £");
            Map(m => m.July).Index(i++).Name($"July 20{startYear} £");
            Map(m => m.August).Index(i++).Name($"August 20{startYear} £");
            Map(m => m.September).Index(i++).Name($"September 20{startYear} £");
            Map(m => m.October).Index(i++).Name($"October 20{startYear} £");
            Map(m => m.November).Index(i++).Name($"November 20{startYear} £");
            Map(m => m.December).Index(i++).Name($"December 20{startYear} £");
            Map(m => m.January).Index(i++).Name($"January 20{endYear} £");
            Map(m => m.February).Index(i++).Name($"February 20{endYear} £");
            Map(m => m.March).Index(i++).Name($"March 20{endYear} £");
            Map(m => m.OfficialSensitive).Index(i).Name("OFFICIAL-SENSITIVE");
        }
    }
}
