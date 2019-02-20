using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CrossLoad.Dto;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.Queueing.Interface;

namespace ESFA.DC.NCS.Stateless
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ILogger _logger;

        public MessageHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<IQueueCallbackResult> Callback(MessageCrossLoadDcftToDctDto message, IDictionary<string, object> messageProperties, CancellationToken cancellationToken)
        {
            try
            {
                Debug.WriteLine("Entered Callback method");

                if (string.IsNullOrEmpty(message.ErrorMessage))
                {
                    var touchPointId = GetTouchPointIdFromMessage(message);

                    // TODO: run tasks
                }
                else
                {
                    _logger.LogWarning($"NCS job failed for Job Id {message.JobId} because of {message.ErrorMessage}");

                    // TODO: update job status to failed
                }

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
