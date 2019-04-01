using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Interfaces.Service;

namespace ESFA.DC.NCS.Service.Tasks
{
    public class ReportingTask : INcsDataTask
    {
        private readonly ILogger _logger;
        private readonly IReportingController _reportingController;

        public ReportingTask(ILogger logger, IReportingController reportingController)
        {
            _logger = logger;
            _reportingController = reportingController;
        }

        public string TaskName => TaskNameConstants.ReportingTaskName;

        public async Task ExecuteAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Debug.WriteLine("Entered Reporting Task");
        }
    }
}
