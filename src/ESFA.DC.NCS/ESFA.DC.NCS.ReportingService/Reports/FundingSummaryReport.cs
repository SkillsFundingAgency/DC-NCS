using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.IO;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.ReportingService.Reports
{
    public class FundingSummaryReport : AbstractReportBuilder, IModelReport
    {
        private const string ProviderNameCell = "B1";
        private const string TouchpointIdCell = "B2";
        private const string LastNcsUpdateCell = "B3";
        private const string ReportGeneratedAtCell = "B22";

        private readonly IFundingSummaryReportBuilder _builder;
        private readonly IStreamProviderService _streamProviderService;
        private readonly ILogger _logger;
        private readonly string[] _columns = { "OutcomeName", "AprilNumbers", "AprilFunding", "MayNumbers", "MayFunding", "JuneNumbers", "JuneFunding", "JulyNumbers", "JulyFunding", "AugustNumbers", "AugustFunding", "SeptemberNumbers", "SeptemberFunding", "OctoberNumbers", "OctoberFunding", "NovemberNumbers", "NovemberFunding", "DecemberNumbers", "DecemberFunding", "JanuaryNumbers", "JanuaryFunding", "FebruaryNumbers", "FebruaryFunding", "MarchNumbers", "MarchFunding", "TotalNumbers", "TotalFunding" };

        public FundingSummaryReport(IFundingSummaryReportBuilder builder, IStreamProviderService streamProviderService, ILogger logger)
        {
            _builder = builder;
            _streamProviderService = streamProviderService;
            _logger = logger;
            ReportFileName = "Funding Summary Report";
        }

        public async Task GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInfo("Generating Funding Summary Report");

            var fileName = GetFilename(ncsJobContextMessage.DssTimeStamp);
            var manifestResourceStream = _streamProviderService.GetStreamFromTemplate(ReportTemplateConstants.FundingSummaryReport);

            Workbook workbook = new Workbook(manifestResourceStream);
            PopulateWorkbook(workbook, data);

            using (var stream = _streamProviderService.GetStream(archive, $"{fileName}.xlsx"))
            {
                workbook.Save(stream, SaveFormat.Xlsx);
            }

            _logger.LogInfo("Funding Summary Report generated");
        }

        private Workbook PopulateWorkbook(Workbook workbook, IEnumerable<ReportDataModel> data)
        {
            var priorityGroupRows = _builder.BuildPriorityGroupRows(data);
            var nonPriorityGroupRows = _builder.BuildNonPriorityGroupRows(data);

            Worksheet sheet = workbook.Worksheets[0];
            Cells cells = sheet.Cells;

            WriteExcelRows(sheet, priorityGroupRows, _columns, 7);
            WriteExcelRows(sheet, nonPriorityGroupRows, _columns, 15);

            // Clean up blank rows ImportCustomObject inserts
            cells.DeleteRow(13);
            cells.DeleteRow(20);
            cells.DeleteRow(12);
            cells.DeleteRow(18);

            return workbook;
        }
    }
}
