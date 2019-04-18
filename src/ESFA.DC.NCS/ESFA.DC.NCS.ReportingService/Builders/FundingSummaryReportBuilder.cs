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

        private static FundingSummaryReportModel BuildRow(IList<ReportDataModel> data, int outcomeType, string outcomeName)
        {
            return new FundingSummaryReportModel
            {
                OutcomeName = outcomeName,
                AprilNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.April)),
                AprilFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.April)).Select(d => d.Value).Sum(),
                MayNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.May)),
                MayFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.May)).Select(d => d.Value).Sum(),
                JuneNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.June)),
                JuneFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.June)).Select(d => d.Value).Sum(),
                JulyNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.July)),
                JulyFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.July)).Select(d => d.Value).Sum(),
                AugustNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.August)),
                AugustFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.August)).Select(d => d.Value).Sum(),
                SeptemberNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.September)),
                SeptemberFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.September)).Select(d => d.Value).Sum(),
                OctoberNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.October)),
                OctoberFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.October)).Select(d => d.Value).Sum(),
                NovemberNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.November)),
                NovemberFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.November)).Select(d => d.Value).Sum(),
                DecemberNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.December)),
                DecemberFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.December)).Select(d => d.Value).Sum(),
                JanuaryNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.January)),
                JanuaryFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.January)).Select(d => d.Value).Sum(),
                FebruaryNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.February)),
                FebruaryFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.February)).Select(d => d.Value).Sum(),
                MarchNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.March)),
                MarchFunding = data.Where(d => d.OutcomeType.Equals(outcomeType) && d.Period.Equals(MonthsConstants.March)).Select(d => d.Value).Sum(),
                TotalNumbers = data.Count(d => d.OutcomeType.Equals(outcomeType)),
                TotalFunding = data.Where(d => d.OutcomeType.Equals(outcomeType)).Select(d => d.Value).Sum()
            };
        }
    }
}
