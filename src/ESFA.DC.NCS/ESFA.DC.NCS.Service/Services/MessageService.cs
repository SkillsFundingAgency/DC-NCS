using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models;
using ESFA.DC.Queueing.Interface;

namespace ESFA.DC.NCS.Service.Services
{
    public class MessageService : IMessageService
    {
        private readonly ILogger _logger;
        private readonly IQueuePublishService<DssPublishMessageModel> _queuePublishService;

        public MessageService(ILogger logger, IQueuePublishService<DssPublishMessageModel> queuePublishService)
        {
            _logger = logger;
            _queuePublishService = queuePublishService;
        }

        public async Task PublishNcsSuccessMessage(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var message = new DssPublishMessageModel
            {
                Status = MessageConstants.Success,
                JobId = ncsJobContextMessage.DssJobId
            };

            await _queuePublishService.PublishAsync(message);

            _logger.LogInfo("Success message sent to Ncs");
        }
    }
}
