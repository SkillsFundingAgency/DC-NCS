using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.IO;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Interfaces.Service;
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
            var reportData = new List<ReportDataModel>();
            var headerData = new FundingSummaryReportHeaderModel();
            var fundingSummaryData = new List<FundingSummaryReportModel>();
            var reportName = "Funding Summary Report";
            var ukprn = 12345678;
            var jobId = 100;
            var filename = $"12345678/100/Funding Summary Report-{new DateTime():yyyyMMdd-HHmmss}.xlsx";
            var container = "container";

            var loggerMock = new Mock<ILogger>();

            var ncsMessageMock = new Mock<INcsJobContextMessage>();
            ncsMessageMock.Setup(nmm => nmm.DssTimeStamp).Returns(new DateTime(2020, 03, 31));
            ncsMessageMock.Setup(nmm => nmm.Ukprn).Returns(ukprn);
            ncsMessageMock.Setup(nmm => nmm.JobId).Returns(jobId);
            ncsMessageMock.Setup(nmm => nmm.DctContainer).Returns(container);

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(dt => dt.GetNowUtc()).Returns(new DateTime(2019, 01, 01));
            dateTimeProviderMock.Setup(dt => dt.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(new DateTime(2019, 01, 01));

            var streamProviderMock = new Mock<IStreamProviderService>();
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(ReportTemplateConstants.FundingSummaryReport));
            var stream = assembly.GetManifestResourceStream(resourceName);
            streamProviderMock.Setup(spm => spm.GetStreamFromTemplate(ReportTemplateConstants.FundingSummaryReport)).Returns(stream);
            streamProviderMock.Setup(spm => spm.GetStream(It.IsAny<ZipArchive>(), It.IsAny<string>())).Returns(stream);

            var builderMock = new Mock<IFundingSummaryReportBuilder>();
            builderMock.Setup(bm => bm.BuildHeaderData(It.IsAny<INcsJobContextMessage>(), CancellationToken.None)).Returns(headerData);
            builderMock.Setup(bm => bm.BuildPriorityGroupRows(It.IsAny<IEnumerable<ReportDataModel>>())).Returns(fundingSummaryData);
            builderMock.Setup(bm => bm.BuildNonPriorityGroupRows(It.IsAny<IEnumerable<ReportDataModel>>())).Returns(fundingSummaryData);

            var fileNameServiceMock = new Mock<IFilenameService>();
            fileNameServiceMock.Setup(fns => fns.GetFilename(ukprn, jobId, reportName, It.IsAny<DateTime>(), OutputTypes.Excel)).Returns(filename);

            var excelServiceMock = new Mock<IExcelService>();

            var report = NewService(builderMock.Object, streamProviderMock.Object, dateTimeProviderMock.Object, loggerMock.Object, excelServiceMock.Object, fileNameServiceMock.Object);

            var result = await report.GenerateReport(reportData, ncsMessageMock.Object, CancellationToken.None);

            result.First().Should().Be(filename);
            result.Should().HaveCount(1);
        }

        private FundingSummaryReport NewService(
            IFundingSummaryReportBuilder builder = null, 
            IStreamProviderService streamProviderService = null, 
            IDateTimeProvider dateTimeProvider = null, 
            ILogger logger = null, 
            IExcelService excelService = null, 
            IFilenameService filenameService = null)
        {
            return new FundingSummaryReport(builder, streamProviderService, dateTimeProvider, logger, excelService, filenameService);
        }
    }
}
