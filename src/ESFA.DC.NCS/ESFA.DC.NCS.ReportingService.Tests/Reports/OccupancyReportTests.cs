using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.ReportingService.Reports;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.NCS.ReportingService.Tests.Reports
{
    public class OccupancyReportTests
    {
        [Fact]
        public void GetCsvTest_ShouldNotBeNull()
        {
            var reportData = new List<OccupancyReportModel>()
            {
                new OccupancyReportModel()
                {
                    CustomerId = Guid.NewGuid(),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    ActionPlanId = Guid.NewGuid(),
                    SessionDate = new DateTime(2000, 02, 02),
                    SubContractorId = "123",
                    AdviserName = "Adviser",
                    OutcomeId = Guid.NewGuid(),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2000, 03, 03),
                    OutcomePriorityCustomer = 0,
                    Value = 10,
                    Period = "July"
                }
            };

            var csvData = NewService().GetCsv(reportData, CancellationToken.None);

            csvData.Should().NotBeNullOrWhiteSpace();
            csvData.Should().Contain(reportData.Single().CustomerId.ToString());
            csvData.Should().Contain(reportData.Single().DateOfBirth.ToString("dd/MM/yyyy"));
            csvData.Should().Contain(reportData.Single().HomePostCode);
            csvData.Should().Contain(reportData.Single().ActionPlanId.ToString());
            csvData.Should().Contain(reportData.Single().SessionDate.ToString("dd/MM/yyyy"));
            csvData.Should().Contain(reportData.Single().SubContractorId);
            csvData.Should().Contain(reportData.Single().AdviserName);
            csvData.Should().Contain(reportData.Single().OutcomeId.ToString());
            csvData.Should().Contain(reportData.Single().OutcomeType.ToString());
            csvData.Should().Contain(reportData.Single().OutcomeEffectiveDate.ToString("dd/MM/yyyy"));
            csvData.Should().Contain(reportData.Single().OutcomePriorityCustomer.ToString());
            csvData.Should().Contain(reportData.Single().Value.ToString());
            csvData.Should().Contain(reportData.Single().Period);
        }

        private OccupancyReport NewService(ILogger logger = null)
        {
            return new OccupancyReport(logger);
        }
    }
}
