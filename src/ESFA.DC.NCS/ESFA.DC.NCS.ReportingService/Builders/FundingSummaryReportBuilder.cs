using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Models.Reports;
using ESFA.DC.NCS.Models.Reports.FundingSummaryReport;

namespace ESFA.DC.NCS.ReportingService.Builders
{
    public class FundingSummaryReportBuilder : IFundingSummaryReportBuilder
    {
        private readonly IOrgDataService _orgDataService;
        private readonly ISourceQueryService _sourceQueryService;

        public FundingSummaryReportBuilder(IOrgDataService orgDataService, ISourceQueryService sourceQueryService)
        {
            _orgDataService = orgDataService;
            _sourceQueryService = sourceQueryService;
        }

        public FundingSummaryReportHeaderModel BuildHeaderData(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            var lastNcsSubmissionDate = _sourceQueryService.GetLastNcsSubmissionDate(ncsJobContextMessage, cancellationToken);

            return new FundingSummaryReportHeaderModel
            {
                ProviderName = _orgDataService.GetProviderName(ncsJobContextMessage.Ukprn, cancellationToken),
                TouchpointId = ncsJobContextMessage.TouchpointId,
                LastNcsUpdate = lastNcsSubmissionDate != null
                                ? lastNcsSubmissionDate.Value.ToString("dd/MM/yyyy hh:mm:ss")
                                : "Unknown"
            };
        }

        public IEnumerable<FundingSummaryReportModel> BuildPriorityGroupRows(IEnumerable<ReportDataModel> data)
        {
            var priorityGroupRecords = data.Where(d => d.OutcomePriorityCustomer.Equals(OutcomeRatesConstants.Priority)).ToList();

            return new List<FundingSummaryReportModel>
            {
                BuildRow(priorityGroupRecords, OutcomeTypesConstants.CustomerSatisfaction, "Customer Satisfaction Priority Group Outcomes"),
                BuildRow(priorityGroupRecords, OutcomeTypesConstants.CareerManagement, "Career Management Priority Group Outcomes"),
                BuildRow(priorityGroupRecords, OutcomeTypesConstants.JobsAndLearning[0], "Sustainable Employment Priority Group Outcomes"),
                BuildRow(priorityGroupRecords, OutcomeTypesConstants.JobsAndLearning[1], "Accredited Learning Priority Group Outcomes"),
                BuildRow(priorityGroupRecords, OutcomeTypesConstants.JobsAndLearning[2], "Career Progression Priority Group Outcomes"),
            };
        }

        public IEnumerable<FundingSummaryReportModel> BuildNonPriorityGroupRows(IEnumerable<ReportDataModel> data)
        {
            var nonPriorityGroupRecords = data.Where(d => d.OutcomePriorityCustomer.Equals(OutcomeRatesConstants.NonPriority)).ToList();

            return new List<FundingSummaryReportModel>
            {
                BuildRow(nonPriorityGroupRecords, OutcomeTypesConstants.CustomerSatisfaction, "Customer Satisfaction Non-Priority Group Outcomes"),
                BuildRow(nonPriorityGroupRecords, OutcomeTypesConstants.CareerManagement, "Career Management Non-Priority Group Outcomes"),
                BuildRow(nonPriorityGroupRecords, OutcomeTypesConstants.JobsAndLearning[0], "Sustainable Employment Non-Priority Group Outcomes"),
                BuildRow(nonPriorityGroupRecords, OutcomeTypesConstants.JobsAndLearning[1], "Accredited Learning Non-Priority Group Outcomes"),
                BuildRow(nonPriorityGroupRecords, OutcomeTypesConstants.JobsAndLearning[2], "Career Progression Non-Priority Group Outcomes"),
            };
        }

        private static FundingSummaryReportModel BuildRow(IEnumerable<ReportDataModel> data, int outcomeType, string outcomeName)
        {
            var outcomeData = data.Where(d => d.OutcomeType.Equals(outcomeType)).ToList();

            ILookup<string, ReportDataModel> lookup = outcomeData.ToLookup(d => d.Period);

            return new FundingSummaryReportModel
            {
                OutcomeName = outcomeName,
                AprilNumbers = lookup[MonthsConstants.April].Count(),
                AprilFunding = lookup[MonthsConstants.April].Sum(d => d.Value),
                MayNumbers = lookup[MonthsConstants.May].Count(),
                MayFunding = lookup[MonthsConstants.May].Sum(d => d.Value),
                JuneNumbers = lookup[MonthsConstants.June].Count(),
                JuneFunding = lookup[MonthsConstants.June].Sum(d => d.Value),
                JulyNumbers = lookup[MonthsConstants.July].Count(),
                JulyFunding = lookup[MonthsConstants.July].Sum(d => d.Value),
                AugustNumbers = lookup[MonthsConstants.August].Count(),
                AugustFunding = lookup[MonthsConstants.August].Sum(d => d.Value),
                SeptemberNumbers = lookup[MonthsConstants.September].Count(),
                SeptemberFunding = lookup[MonthsConstants.September].Sum(d => d.Value),
                OctoberNumbers = lookup[MonthsConstants.October].Count(),
                OctoberFunding = lookup[MonthsConstants.October].Sum(d => d.Value),
                NovemberNumbers = lookup[MonthsConstants.November].Count(),
                NovemberFunding = lookup[MonthsConstants.November].Sum(d => d.Value),
                DecemberNumbers = lookup[MonthsConstants.December].Count(),
                DecemberFunding = lookup[MonthsConstants.December].Sum(d => d.Value),
                JanuaryNumbers = lookup[MonthsConstants.January].Count(),
                JanuaryFunding = lookup[MonthsConstants.January].Sum(d => d.Value),
                FebruaryNumbers = lookup[MonthsConstants.February].Count(),
                FebruaryFunding = lookup[MonthsConstants.February].Sum(d => d.Value),
                MarchNumbers = lookup[MonthsConstants.March].Count(),
                MarchFunding = lookup[MonthsConstants.March].Sum(d => d.Value),
                TotalNumbers = outcomeData.Count,
                TotalFunding = outcomeData.Sum(d => d.Value)
            };
        }
    }
}
