using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.Service;

namespace ESFA.DC.NCS.Service.Tasks
{
    public class StorageTask : INcsDataTask
    {
        private readonly ILogger _logger;

        public StorageTask(ILogger logger)
        {
            _logger = logger;
        }

        public string TaskName => TaskNameConstants.StorageTaskName;

        public async Task ExecuteAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Debug.WriteLine("Entered Storage Task");
        }
    }
}
