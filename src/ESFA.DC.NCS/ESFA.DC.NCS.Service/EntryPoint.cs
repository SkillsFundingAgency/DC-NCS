using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Service;

namespace ESFA.DC.NCS.Service
{
    public class EntryPoint : IEntryPoint
    {
        private readonly ILogger _logger;

        public EntryPoint(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> CallbackAsync(IEnumerable<INcsDataTask> ncsTasks, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            Debug.WriteLine("Entered EntryPoint");
            _logger.LogInfo("NCS callback invoked");

            foreach (var task in ncsTasks)
            {
                _logger.LogInfo($"NCS Service Task : {task.TaskName} Starting");
                await task.ExecuteAsync(ncsJobContextMessage, cancellationToken);
                _logger.LogInfo($"NCS Service Task : {task.TaskName} Finished");
            }

            return true;
        }
    }
}
