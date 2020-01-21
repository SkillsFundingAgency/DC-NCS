﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
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
        private readonly IFundingSummaryReportBuilder _builder;
        private readonly IStreamProviderService _streamProviderService;
        private readonly ILogger _logger;
        private readonly IExcelService _excelService;
        private readonly IFilenameService _filenameService;
        private readonly string[] _columns = { "OutcomeName", "AprilNumbers", "AprilFunding", "MayNumbers", "MayFunding", "JuneNumbers", "JuneFunding", "JulyNumbers", "JulyFunding", "AugustNumbers", "AugustFunding", "SeptemberNumbers", "SeptemberFunding", "OctoberNumbers", "OctoberFunding", "NovemberNumbers", "NovemberFunding", "DecemberNumbers", "DecemberFunding", "JanuaryNumbers", "JanuaryFunding", "FebruaryNumbers", "FebruaryFunding", "MarchNumbers", "MarchFunding", "TotalNumbers", "TotalFunding" };

        public FundingSummaryReport(IFundingSummaryReportBuilder builder, IStreamProviderService streamProviderService, ILogger logger, IExcelService excelService, IFilenameService filenameService)
        {
            _builder = builder;
            _streamProviderService = streamProviderService;
            _logger = logger;
            _excelService = excelService;
            _filenameService = filenameService;
        }

        public async Task<string[]> GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Generating Funding Summary Report");

            var fileName = _filenameService.GetFilename(ncsJobContextMessage.Ukprn, ncsJobContextMessage.JobId, ReportNameConstants.FundingSummary, ncsJobContextMessage.DssTimeStamp, OutputTypes.Excel);
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
            var footerData = _builder.BuildFooterData(cancellationToken);

            Worksheet sheet = workbook.Worksheets[0];

            // Header
            RenderDictionary(sheet, 0, headerData);

            // Body
            _excelService.WriteExcelRows(sheet, priorityGroupRows, _columns, 7);
            _excelService.WriteExcelRows(sheet, nonPriorityGroupRows, _columns, 13);

            // Footer
            RenderDictionary(sheet, 21, footerData);
        }

        private void RenderDictionary(Worksheet worksheet, int row, IDictionary<string, string> data)
        {
            foreach (var entry in data)
            {
                worksheet.Cells.ImportTwoDimensionArray(
                    new object[,]
                    {
                        { entry.Key, entry.Value }
                    },
                    row,
                    0);

                row++;
            }
        }

        private IEnumerable<ReportDataModel> GetReportData(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage)
        {
            return data.Where(d => d.OutcomeEffectiveDate <= ncsJobContextMessage.DssTimeStamp);
        }
    }
}
