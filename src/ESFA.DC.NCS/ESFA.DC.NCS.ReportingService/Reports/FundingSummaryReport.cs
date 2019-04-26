using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.DateTimeProvider.Interface;
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
        private const string SecurityClassificationCell = "B4";
        private const string ReportGeneratedAtCell = "B22";

        private readonly IFundingSummaryReportBuilder _builder;
        private readonly IStreamProviderService _streamProviderService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;
        private readonly string[] _columns = { "OutcomeName", "AprilNumbers", "AprilFunding", "MayNumbers", "MayFunding", "JuneNumbers", "JuneFunding", "JulyNumbers", "JulyFunding", "AugustNumbers", "AugustFunding", "SeptemberNumbers", "SeptemberFunding", "OctoberNumbers", "OctoberFunding", "NovemberNumbers", "NovemberFunding", "DecemberNumbers", "DecemberFunding", "JanuaryNumbers", "JanuaryFunding", "FebruaryNumbers", "FebruaryFunding", "MarchNumbers", "MarchFunding", "TotalNumbers", "TotalFunding" };

        public FundingSummaryReport(IFundingSummaryReportBuilder builder, IStreamProviderService streamProviderService, IDateTimeProvider dateTimeProvider, ILogger logger)
        {
            _builder = builder;
            _streamProviderService = streamProviderService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            ReportFileName = "Funding Summary Report";
        }

        public async Task GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInfo("Generating Funding Summary Report");

            var fileName = GetFilename(ncsJobContextMessage.DssTimeStamp);
            var manifestResourceStream = _streamProviderService.GetStreamFromTemplate(ReportTemplateConstants.FundingSummaryReport);
            var reportData = GetReportData(data, ncsJobContextMessage);

            Workbook workbook = new Workbook(manifestResourceStream);
            PopulateWorkbook(workbook, ncsJobContextMessage, reportData, cancellationToken);

            using (var stream = _streamProviderService.GetStream(archive, $"{fileName}.xlsx"))
            {
                workbook.Save(stream, SaveFormat.Xlsx);
            }

            _logger.LogInfo("Funding Summary Report generated");
        }

        private void PopulateWorkbook(Workbook workbook, INcsJobContextMessage ncsJobContextMessage, IEnumerable<ReportDataModel> data, CancellationToken cancellationToken)
        {
            var headerData = _builder.BuildHeaderData(ncsJobContextMessage, cancellationToken);
            var priorityGroupRows = _builder.BuildPriorityGroupRows(data);
            var nonPriorityGroupRows = _builder.BuildNonPriorityGroupRows(data);

            Worksheet sheet = workbook.Worksheets[0];
            Cells cells = sheet.Cells;

            // Header
            cells[ProviderNameCell].PutValue(headerData.ProviderName);
            cells[TouchpointIdCell].PutValue(headerData.TouchpointId);
            cells[LastNcsUpdateCell].PutValue(headerData.LastNcsUpdate);
            cells[SecurityClassificationCell].PutValue(headerData.SecurityClassification);

            // Body
            WriteExcelRows(sheet, priorityGroupRows, _columns, 7);
            WriteExcelRows(sheet, nonPriorityGroupRows, _columns, 15);

            // Clean up blank rows ImportCustomObject inserts
            CleanUp(cells);

            // Footer
            var dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            var dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc).ToString("dd/MM/yyyy hh:mm:ss");

            cells[ReportGeneratedAtCell].PutValue(dateTimeNowUk);
        }

        private void CleanUp(Cells cells)
        {
            cells.DeleteRow(13);
            cells.DeleteRow(20);
            cells.DeleteRow(12);
            cells.DeleteRow(18);
        }

        private IEnumerable<ReportDataModel> GetReportData(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage)
        {
            // TODO: Need clarification on the date to filter on - waiting for collection dates
            return data.Where(d => d.OutcomeEffectiveDate <= ncsJobContextMessage.DssTimeStamp);
        }
    }
}
