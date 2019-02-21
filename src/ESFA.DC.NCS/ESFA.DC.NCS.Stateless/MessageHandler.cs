using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CrossLoad.Dto;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Models.Messages;
using ESFA.DC.Queueing.Interface;

namespace ESFA.DC.NCS.Stateless
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ILogger _logger;
        private readonly IQueuePublishService<DssPublishMessageModel> _queuePublishService;

        public MessageHandler(ILogger logger, IQueuePublishService<DssPublishMessageModel> queuePublishService)
        {
            _logger = logger;
            _queuePublishService = queuePublishService;
        }

        public async Task<IQueueCallbackResult> Callback(MessageCrossLoadDcftToDctDto message, IDictionary<string, object> messageProperties, CancellationToken cancellationToken)
        {
            try
            {
                Debug.WriteLine("Entered Callback method");

                // TODO: run tasks

                var publishMessage = new DssPublishMessageModel
                {
                    TestProperty = "Model structure TBC"
                };

                await _queuePublishService.PublishAsync(publishMessage);

                return new QueueCallbackResult(true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"NCS failed to post status update for Job Id {message.JobId}", ex);
                return new QueueCallbackResult(false, ex);
            }
        }

        private string GetTouchPointIdFromMessage(MessageCrossLoadDcftToDctDto message)
        {
            // TODO: Get touchPointId from file name in message???

            return string.Empty;
        }
    }
}
