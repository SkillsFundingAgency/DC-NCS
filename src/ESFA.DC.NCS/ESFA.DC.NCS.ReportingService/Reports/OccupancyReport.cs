using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Models.Reports;
using ESFA.DC.NCS.ReportingService.Mappers;

namespace ESFA.DC.NCS.ReportingService.Reports
{
    public class OccupancyReport : AbstractReportBuilder, IModelReport
    {
        private readonly ILogger _logger;

        public OccupancyReport(ILogger logger)
        {
            _logger = logger;
            ReportFileName = "NCS Occupancy Report";
        }

        public async Task GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInfo("Generating Occupancy Report");

            var fileName = GetFilename(ncsJobContextMessage.DssTimeStamp);

            var reportData = GetOccupancyReportModel(data);

            string csv = GetCsv(reportData, cancellationToken);

            if (string.IsNullOrWhiteSpace(csv))
            {
                _logger.LogInfo("Occupancy Report not generated, no data available");
                return;
            }

            await WriteZipEntry(archive, $"{fileName}.csv", csv);
            _logger.LogInfo("Occupancy Report generated");
        }

        public string GetCsv(IEnumerable<OccupancyReportModel> reportData, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ms = new MemoryStream())
            {
                var utF8Encoding = new UTF8Encoding(true, true);
                using (var textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (var csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<OccupancyReportMapper, OccupancyReportModel>(csvWriter, reportData);

                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }

        private IEnumerable<OccupancyReportModel> GetOccupancyReportModel(IEnumerable<ReportDataModel> data)
        {
            var occupancyReportData = new List<OccupancyReportModel>();

            foreach (var item in data)
            {
                var reportData = new OccupancyReportModel()
                {
                    CustomerId = item.CustomerId,
                    DateOfBirth = item.DateOfBirth,
                    HomePostCode = item.HomePostCode,
                    ActionPlanId = item.ActionPlanId,
                    SessionDate = item.SessionDate,
                    SubContractorId = item.SubContractorId,
                    AdviserName = item.AdviserName,
                    OutcomeId = item.OutcomeId,
                    OutcomeType = item.OutcomeType,
                    OutcomeEffectiveDate = item.OutcomeEffectiveDate,
                    OutcomePriorityCustomer = item.OutcomePriorityCustomer,
                    Period = item.Period,
                    Value = item.Value
                };

                occupancyReportData.Add(reportData);
            }

            return occupancyReportData;
        }
    }
}
