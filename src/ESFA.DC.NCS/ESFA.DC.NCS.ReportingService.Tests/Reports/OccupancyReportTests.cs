using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Service;
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
        public async Task GenerateReport()
        {
            var ukprn = 12345678;
            var jobId = 100;
            var reportName = "NCS Occupancy Report";
            var filename = $"12345678/100/NCS Occupancy Report-{new DateTime():yyyyMMdd-HHmmss}.csv";
            var container = "container";
            var collectionYear= 1920;
            var reportData = new List<ReportDataModel>();

            var ncsMessageMock = new Mock<INcsJobContextMessage>();
            ncsMessageMock.Setup(nmm => nmm.DssTimeStamp).Returns(new DateTime(2020, 03, 31));
            ncsMessageMock.Setup(nmm => nmm.ReportEndDate).Returns(new DateTime(2020, 03, 31));
            ncsMessageMock.Setup(nmm => nmm.Ukprn).Returns(ukprn);
            ncsMessageMock.Setup(nmm => nmm.JobId).Returns(jobId);
            ncsMessageMock.Setup(nmm => nmm.DctContainer).Returns(container);

            var classMapFactoryMock = new Mock<IClassMapFactory<OccupancyReportModel>>();
            classMapFactoryMock.Setup(cmf => cmf.Build(ncsMessageMock.Object)).Returns(Mock.Of<ClassMap<OccupancyReportModel>>());

            var loggerMock = new Mock<ILogger>();
            var fileNameServiceMock = new Mock<IFilenameService>();
            var csvServiceMock = new Mock<ICsvFileService>();

            fileNameServiceMock.Setup(fns => fns.GetFilename(ukprn, jobId, reportName, It.IsAny<DateTime>(), OutputTypes.Csv)).Returns(filename);

            var report = NewService(csvServiceMock.Object, fileNameServiceMock.Object, classMapFactoryMock.Object, loggerMock.Object);

            var result = await report.GenerateReport(reportData, ncsMessageMock.Object, CancellationToken.None);

            result.First().Should().Be(filename);
            result.Should().HaveCount(1);
        }

        private OccupancyReport NewService(ICsvFileService csvFileService = null, IFilenameService filenameService = null, IClassMapFactory<OccupancyReportModel> mapFactory = null, ILogger logger = null)
        {
            return new OccupancyReport(csvFileService, filenameService, mapFactory, logger);
        }
    }
}
