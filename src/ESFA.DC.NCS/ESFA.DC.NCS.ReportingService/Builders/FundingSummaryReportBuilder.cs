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

            return new FundingSummaryReportModel
            {
                OutcomeName = outcomeName,
                AprilNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.April)),
                AprilFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.April)).Sum(d => d.Value),
                MayNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.May)),
                MayFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.May)).Sum(d => d.Value),
                JuneNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.June)),
                JuneFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.June)).Sum(d => d.Value),
                JulyNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.July)),
                JulyFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.July)).Sum(d => d.Value),
                AugustNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.August)),
                AugustFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.August)).Sum(d => d.Value),
                SeptemberNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.September)),
                SeptemberFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.September)).Sum(d => d.Value),
                OctoberNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.October)),
                OctoberFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.October)).Sum(d => d.Value),
                NovemberNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.November)),
                NovemberFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.November)).Sum(d => d.Value),
                DecemberNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.December)),
                DecemberFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.December)).Sum(d => d.Value),
                JanuaryNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.January)),
                JanuaryFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.January)).Sum(d => d.Value),
                FebruaryNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.February)),
                FebruaryFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.February)).Sum(d => d.Value),
                MarchNumbers = outcomeData.Count(d => d.Period.Equals(MonthsConstants.March)),
                MarchFunding = outcomeData.Where(d => d.Period.Equals(MonthsConstants.March)).Sum(d => d.Value),
                TotalNumbers = outcomeData.Count,
                TotalFunding = outcomeData.Sum(d => d.Value)
            };
        }
    }
}
