using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.IO;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Models.Reports;
using ESFA.DC.NCS.ReportingService.Mappers;
using ESFA.DC.NCS.ReportingService.Reports;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.NCS.ReportingService.Tests.Reports
{
    public class OccupancyReportTests
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

            var reportName = "OccupancyReport";

            var loggerMock = new Mock<ILogger>();

            var ncsMessageMock = new Mock<INcsJobContextMessage>();
            ncsMessageMock.Setup(nmm => nmm.DssTimeStamp).Returns(new DateTime(2000, 01, 01));

            var streamProviderMock = new Mock<IStreamProviderService>();

            using (var memoryStream = new MemoryStream())
            {
                using (var archiveCreate = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var stream = archiveCreate.CreateEntry(reportName, CompressionLevel.Optimal).Open();

                    streamProviderMock.Setup(spm => spm.GetStream(archiveCreate, It.IsAny<string>())).Returns(stream);

                    await NewService(streamProviderMock.Object, loggerMock.Object).GenerateReport(reportData, ncsMessageMock.Object, archiveCreate, CancellationToken.None);
                }

                using (var archiveRead = new ZipArchive(memoryStream, ZipArchiveMode.Read, true))
                {
                    archiveRead.Entries.Count.Should().Be(1);
                    archiveRead.GetEntry(reportName).Should().NotBeNull();
                }
            }
        }

        [Fact]
        public async Task GenerateReport_OneRecord()
        {
            var dssTimeStamp = new DateTime(2019, 02, 20);

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
                    SessionDate = new DateTime(2019, 01, 01),
                    SubContractorId = "123",
                    AdviserName = "Adviser",
                    OutcomeId = Guid.NewGuid(),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 02, 01),
                    OutcomePriorityCustomer = 0,
                    CollectionYear = 1920,
                    DssJobId = Guid.NewGuid(),
                    DssTimestamp = dssTimeStamp,
                    CreatedOn = new DateTime(2019, 02, 02),
                    Value = 10,
                    Period = "July"
                }
            };

            var reportName = "OccupancyReport";

            var loggerMock = new Mock<ILogger>();

            var ncsMessageMock = new Mock<INcsJobContextMessage>();
            ncsMessageMock.Setup(nmm => nmm.DssTimeStamp).Returns(dssTimeStamp);

            var streamProviderMock = new Mock<IStreamProviderService>();

            using (var memoryStream = new MemoryStream())
            {
                using (var archiveCreate = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var stream = archiveCreate.CreateEntry(reportName, CompressionLevel.Optimal).Open();

                    streamProviderMock.Setup(spm => spm.GetStream(archiveCreate, It.IsAny<string>())).Returns(stream);

                    await NewService(streamProviderMock.Object, loggerMock.Object).GenerateReport(reportData, ncsMessageMock.Object, archiveCreate, CancellationToken.None);
                }

                var records = GetRecords(memoryStream, reportName);
                records.Count.Should().Be(1);
                records.Single().ActionPlanId.Should().Be(reportData.Single().ActionPlanId);
                records.Single().AdviserName.Should().Be(reportData.Single().AdviserName);
                records.Single().CustomerId.Should().Be(reportData.Single().CustomerId);
                records.Single().DateOfBirth.Should().Be(reportData.Single().DateOfBirth);
                records.Single().HomePostCode.Should().Be(reportData.Single().HomePostCode);
                records.Single().OutcomeEffectiveDate.Should().Be(reportData.Single().OutcomeEffectiveDate);
                records.Single().OutcomeId.Should().Be(reportData.Single().OutcomeId);
                records.Single().OutcomePriorityCustomer.Should().Be(reportData.Single().OutcomePriorityCustomer);
                records.Single().OutcomeType.Should().Be(reportData.Single().OutcomeType);
                records.Single().Period.Should().Be(reportData.Single().Period);
                records.Single().SubContractorId.Should().Be(reportData.Single().SubContractorId);
                records.Single().Value.Should().Be(reportData.Single().Value);
            }
        }

        [Fact]
        public async Task GenerateReport_MultipleRecords()
        {
            var dssTimeStamp = new DateTime(2019, 02, 20);

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
                    SessionDate = new DateTime(2019, 01, 01),
                    SubContractorId = "123",
                    AdviserName = "Adviser",
                    OutcomeId = Guid.NewGuid(),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 02, 01),
                    OutcomePriorityCustomer = 0,
                    CollectionYear = 1920,
                    DssJobId = Guid.NewGuid(),
                    DssTimestamp = dssTimeStamp,
                    CreatedOn = new DateTime(2019, 02, 02),
                    Value = 10,
                    Period = "July"
                },
                new ReportDataModel()
                {
                    Ukprn = 123456789,
                    TouchpointId = "9999999999",
                    CustomerId = Guid.NewGuid(),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    ActionPlanId = Guid.NewGuid(),
                    SessionDate = new DateTime(2019, 01, 01),
                    SubContractorId = "123",
                    AdviserName = "Adviser",
                    OutcomeId = Guid.NewGuid(),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 01, 01),
                    OutcomePriorityCustomer = 0,
                    CollectionYear = 1920,
                    DssJobId = Guid.NewGuid(),
                    DssTimestamp = dssTimeStamp,
                    CreatedOn = new DateTime(2019, 02, 02),
                    Value = 10,
                    Period = "July"
                }
            };

            var reportName = "OccupancyReport";

            var loggerMock = new Mock<ILogger>();

            var ncsMessageMock = new Mock<INcsJobContextMessage>();
            ncsMessageMock.Setup(nmm => nmm.DssTimeStamp).Returns(dssTimeStamp);

            var streamProviderMock = new Mock<IStreamProviderService>();

            using (var memoryStream = new MemoryStream())
            {
                using (var archiveCreate = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var stream = archiveCreate.CreateEntry(reportName, CompressionLevel.Optimal).Open();

                    streamProviderMock.Setup(spm => spm.GetStream(archiveCreate, It.IsAny<string>())).Returns(stream);

                    await NewService(streamProviderMock.Object, loggerMock.Object).GenerateReport(reportData, ncsMessageMock.Object, archiveCreate, CancellationToken.None);
                }

                var records = GetRecords(memoryStream, reportName);
                records.Count.Should().Be(2);
            }
        }

        [Fact]
        public async Task GenerateReport_FilterFutureRecords()
        {
            var dssTimeStamp = new DateTime(2019, 02, 20);

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
                    SessionDate = new DateTime(2019, 01, 01),
                    SubContractorId = "123",
                    AdviserName = "Adviser",
                    OutcomeId = Guid.NewGuid(),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 05, 01),
                    OutcomePriorityCustomer = 0,
                    CollectionYear = 1920,
                    DssJobId = Guid.NewGuid(),
                    DssTimestamp = dssTimeStamp,
                    CreatedOn = new DateTime(2019, 02, 02),
                    Value = 10,
                    Period = "July"
                },
                new ReportDataModel()
                {
                    Ukprn = 123456789,
                    TouchpointId = "9999999999",
                    CustomerId = Guid.NewGuid(),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    ActionPlanId = Guid.NewGuid(),
                    SessionDate = new DateTime(2019, 01, 01),
                    SubContractorId = "123",
                    AdviserName = "Adviser",
                    OutcomeId = Guid.NewGuid(),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 06, 01),
                    OutcomePriorityCustomer = 0,
                    CollectionYear = 1920,
                    DssJobId = Guid.NewGuid(),
                    DssTimestamp = dssTimeStamp,
                    CreatedOn = new DateTime(2019, 02, 02),
                    Value = 10,
                    Period = "July"
                }
            };

            var reportName = "OccupancyReport";

            var loggerMock = new Mock<ILogger>();

            var ncsMessageMock = new Mock<INcsJobContextMessage>();
            ncsMessageMock.Setup(nmm => nmm.DssTimeStamp).Returns(dssTimeStamp);

            var streamProviderMock = new Mock<IStreamProviderService>();

            using (var memoryStream = new MemoryStream())
            {
                using (var archiveCreate = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var stream = archiveCreate.CreateEntry(reportName, CompressionLevel.Optimal).Open();

                    streamProviderMock.Setup(spm => spm.GetStream(archiveCreate, It.IsAny<string>())).Returns(stream);

                    await NewService(streamProviderMock.Object, loggerMock.Object).GenerateReport(reportData, ncsMessageMock.Object, archiveCreate, CancellationToken.None);
                }

                var records = GetRecords(memoryStream, reportName);
                records.Count.Should().Be(0);
            }
        }

        private List<OccupancyReportModel> GetRecords(MemoryStream memoryStream, string reportName)
        {
            using (var archiveRead = new ZipArchive(memoryStream, ZipArchiveMode.Read, true))
            using (var file = archiveRead.GetEntry(reportName).Open())
            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.RegisterClassMap(new OccupancyReportMapper());
                return csv.GetRecords<OccupancyReportModel>().ToList();
            }
        }

        private OccupancyReport NewService(IStreamProviderService streamProviderService = null, ILogger logger = null)
        {
            return new OccupancyReport(streamProviderService, logger);
        }
    }
}
