using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.ReportingService.Reports
{
    public class FundingSummaryReport : AbstractReportBuilder, IModelReport
    {
        public async Task GenerateReport(IEnumerable<ReportDataModel> data, INcsJobContextMessage ncsJobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            Debug.WriteLine("Entered Funding Summary Report");
        }
    }
}
