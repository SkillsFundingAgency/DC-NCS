using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.Service;

namespace ESFA.DC.NCS.Service
{
    public class EntryPoint : IEntryPoint
    {
        private readonly ILogger _logger;
        private readonly IMessageService _messageService;

        public EntryPoint(ILogger logger, IMessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        public async Task<bool> CallbackAsync(IEnumerable<INcsDataTask> ncsTasks, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            _logger.LogInfo("NCS callback invoked");

            foreach (var task in ncsTasks)
            {
                _logger.LogInfo($"NCS Service Task : {task.TaskName} Starting");
                await task.ExecuteAsync(ncsJobContextMessage, cancellationToken);
                _logger.LogInfo($"NCS Service Task : {task.TaskName} Finished");
            }

            await _messageService.PublishNcsMessage(MessageConstants.Success, ncsJobContextMessage, cancellationToken);

            return true;
        }
    }
}
