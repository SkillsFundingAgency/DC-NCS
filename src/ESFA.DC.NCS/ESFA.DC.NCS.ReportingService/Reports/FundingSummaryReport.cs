using System.Collections.Generic;
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
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.ReportingService.Reports
{
    public class FundingSummaryReport : IModelReport
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
        private readonly IExcelService _excelService;
        private readonly IFilenameService _filenameService;
        private readonly string[] _columns = { "OutcomeName", "AprilNumbers", "AprilFunding", "MayNumbers", "MayFunding", "JuneNumbers", "JuneFunding", "JulyNumbers", "JulyFunding", "AugustNumbers", "AugustFunding", "SeptemberNumbers", "SeptemberFunding", "OctoberNumbers", "OctoberFunding", "NovemberNumbers", "NovemberFunding", "DecemberNumbers", "DecemberFunding", "JanuaryNumbers", "JanuaryFunding", "FebruaryNumbers", "FebruaryFunding", "MarchNumbers", "MarchFunding", "TotalNumbers", "TotalFunding" };

        public FundingSummaryReport(IFundingSummaryReportBuilder builder, IStreamProviderService streamProviderService, IDateTimeProvider dateTimeProvider, ILogger logger, IExcelService excelService, IFilenameService filenameService)
        {
            _builder = builder;
            _streamProviderService = streamProviderService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _excelService = excelService;
            _filenameService = filenameService;
        }

        public async Task<string[]> GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Generating Funding Summary Report");

            var fileName = _filenameService.GetFilename(ncsJobContextMessage.Ukprn, ncsJobContextMessage.JobId, ReportNameConstants.fundingSummary, ncsJobContextMessage.DssTimeStamp, OutputTypes.Excel);
            var manifestResourceStream = _streamProviderService.GetStreamFromTemplate(ReportTemplateConstants.FundingSummaryReport);
            var reportData = GetReportData(data, ncsJobContextMessage);

            Workbook workbook = new Workbook(manifestResourceStream);
            PopulateWorkbook(workbook, ncsJobContextMessage, reportData, cancellationToken);

            await _excelService.SaveWorkbookAsync(workbook, fileName, ncsJobContextMessage.DctContainer, cancellationToken);

            _logger.LogInfo("Funding Summary Report generated");

            return new[] { fileName };
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
            _excelService.WriteExcelRows(sheet, priorityGroupRows, _columns, 7);
            _excelService.WriteExcelRows(sheet, nonPriorityGroupRows, _columns, 15);

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
