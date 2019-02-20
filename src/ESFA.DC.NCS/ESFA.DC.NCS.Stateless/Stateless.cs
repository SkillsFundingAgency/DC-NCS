using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CrossLoad.Dto;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.Queueing.Interface;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ESFA.DC.NCS.Stateless
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Stateless : StatelessService
    {
        private readonly ILogger _logger;
        private readonly IMessageHandler _messageHandler;
        private readonly IQueueSubscriptionService<MessageCrossLoadDcftToDctDto> _queueSubscriptionService;

        public Stateless(
            StatelessServiceContext context,
            ILogger logger,
            IMessageHandler messageHandler,
            IQueueSubscriptionService<MessageCrossLoadDcftToDctDto> queueSubscriptionService)
            : base(context)
        {
            _logger = logger;
            _messageHandler = messageHandler;
            _queueSubscriptionService = queueSubscriptionService;
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            bool initialised = false;
            try
            {
                _logger.LogInfo("NCS Stateless Service Started");

                _queueSubscriptionService.Subscribe(_messageHandler.Callback, cancellationToken);

                initialised = true;
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (Exception exception) when (!(exception is TaskCanceledException))
            {
                // Ignore, as an exception is only really thrown on cancellation of the token.
                _logger.LogError("NCS Stateless Service Exception", exception);
            }
            finally
            {
                if (initialised)
                {
                    await _queueSubscriptionService.UnsubscribeAsync();
                    _logger.LogInfo("NCS Stateless Service Ended");
                }
            }
        }
    }
}
