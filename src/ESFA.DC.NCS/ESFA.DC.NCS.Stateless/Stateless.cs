using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ESFA.DC.NCS.Stateless
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Stateless : StatelessService
    {
        private readonly IJobContextManager<JobContextMessage> _jobContextManager;
        private readonly ILogger _logger;

        public Stateless(StatelessServiceContext context, IJobContextManager<JobContextMessage> jobContextManager, ILogger logger)
            : base(context)
        {
            _jobContextManager = jobContextManager;
            _logger = logger;
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

                _jobContextManager.OpenAsync(cancellationToken);

                Debug.WriteLine("Started");

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
                    await _jobContextManager.CloseAsync();
                    _logger.LogInfo("NCS Stateless Service Ended");
                }
            }
        }
    }
}
