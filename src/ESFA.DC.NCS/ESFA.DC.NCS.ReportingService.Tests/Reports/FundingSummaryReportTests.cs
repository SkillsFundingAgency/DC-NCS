using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.IO;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Models.Reports;
using ESFA.DC.NCS.Models.Reports.FundingSummaryReport;
using ESFA.DC.NCS.ReportingService.Reports;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.NCS.ReportingService.Tests.Reports
{
    public class FundingSummaryReportTests
    {
        [Fact]
        public async Task GenerateReport_Success()
        {
            var reportData = new List<ReportDataModel>()
            {
                new ReportDataModel()
                {
                    Ukprn = 123456789,
                    TouchpointId = "9999999999",
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
                    CollectionYear = 1920,
                    DssJobId = Guid.NewGuid(),
                    DssTimestamp = new DateTime(2000, 01, 01),
                    CreatedOn = new DateTime(2000, 04, 01),
                    Value = 10,
                    Period = "July"
                }
            };

            var headerData = new FundingSummaryReportHeaderModel
            {
                ProviderName = "Test",
                TouchpointId = "9999999999",
                LastNcsUpdate = "Unknown"
            };

            var fundingSummaryData = new List<FundingSummaryReportModel>()
            {
                new FundingSummaryReportModel()
                {
                    OutcomeName = "Test",
                    AprilFunding = 1,
                    AprilNumbers = 2,
                    MayNumbers = 3,
                    MayFunding = 4,
                    JuneFunding = 5,
                    JuneNumbers = 6,
                    JulyFunding = 7, 
                    JulyNumbers = 8,
                    AugustFunding = 9,
                    AugustNumbers = 10,
                    SeptemberFunding = 11,
                    SeptemberNumbers = 12,
                    OctoberFunding = 13,
                    OctoberNumbers = 14,
                    NovemberFunding = 15,
                    NovemberNumbers = 16,
                    DecemberFunding = 17,
                    DecemberNumbers = 18,
                    JanuaryFunding = 19,
                    JanuaryNumbers = 20,
                    FebruaryFunding = 21,
                    FebruaryNumbers = 22,
                    MarchFunding = 23,
                    MarchNumbers = 24,
                    TotalFunding = 25,
                    TotalNumbers = 26
                }
            };

            var reportName = "FundingSummaryReport";

            var loggerMock = new Mock<ILogger>();

            var ncsMessageMock = new Mock<INcsJobContextMessage>();
            ncsMessageMock.Setup(nmm => nmm.DssTimeStamp).Returns(new DateTime(2020, 03, 31));

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(dt => dt.GetNowUtc()).Returns(new DateTime(2019, 01, 01));
            dateTimeProviderMock.Setup(dt => dt.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(new DateTime(2019, 01, 01));

            var streamProviderMock = new Mock<IStreamProviderService>();
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(ReportTemplateConstants.FundingSummaryReport));
            var stream = assembly.GetManifestResourceStream(resourceName);

            var builderMock = new Mock<IFundingSummaryReportBuilder>();
            builderMock.Setup(bm => bm.BuildHeaderData(It.IsAny<INcsJobContextMessage>(), CancellationToken.None)).Returns(headerData);
            builderMock.Setup(bm => bm.BuildPriorityGroupRows(It.IsAny<IEnumerable<ReportDataModel>>())).Returns(fundingSummaryData);
            builderMock.Setup(bm => bm.BuildNonPriorityGroupRows(It.IsAny<IEnumerable<ReportDataModel>>())).Returns(fundingSummaryData);

            using (var memoryStream = new MemoryStream())
            {
                using (var archiveCreate = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var streamOpen = archiveCreate.CreateEntry(reportName, CompressionLevel.Optimal).Open();
                    streamProviderMock.Setup(spm => spm.GetStreamFromTemplate(ReportTemplateConstants.FundingSummaryReport)).Returns(stream);
                    streamProviderMock.Setup(spm => spm.GetStream(archiveCreate, It.IsAny<string>())).Returns(streamOpen);

                    await NewService(builderMock.Object, streamProviderMock.Object, dateTimeProviderMock.Object, loggerMock.Object)
                        .GenerateReport(reportData, ncsMessageMock.Object, archiveCreate, CancellationToken.None);
                }

                using (var archiveRead = new ZipArchive(memoryStream, ZipArchiveMode.Read, true))
                {
                    archiveRead.Entries.Count.Should().Be(1);
                    archiveRead.GetEntry(reportName).Should().NotBeNull();
                }
            }
        }

        private FundingSummaryReport NewService(IFundingSummaryReportBuilder builder = null, IStreamProviderService streamProviderService = null, IDateTimeProvider dateTimeProvider = null, ILogger logger = null)
        {
            return new FundingSummaryReport(builder, streamProviderService, dateTimeProvider, logger);
        }
    }
}
