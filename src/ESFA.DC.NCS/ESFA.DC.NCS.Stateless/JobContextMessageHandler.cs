using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Stateless.Context;

namespace ESFA.DC.NCS.Stateless
{
    public class JobContextMessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly ILifetimeScope _lifetimeScope;

        public JobContextMessageHandler(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public async Task<bool> HandleAsync(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            using (var childLifetimeScope = _lifetimeScope.BeginLifetimeScope())
            {
                var executionContext = (Logging.ExecutionContext)childLifetimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = jobContextMessage.JobId.ToString();

                var logger = childLifetimeScope.Resolve<ILogger>();

                var taskNames = GetTaskNamesForTopicFromMessage(jobContextMessage);
                var ncsServiceTasks = childLifetimeScope.Resolve<IEnumerable<INcsDataTask>>();

                try
                {
                    logger.LogDebug($"Started NCS Service for jobId:{jobContextMessage.JobId}");

                    var serviceTasks = ncsServiceTasks.ToList();
                    var tasks = serviceTasks.Where(t => taskNames.Contains(t.TaskName)).ToList();

                    if (!tasks.Any())
                    {
                        logger.LogInfo("NCS. No tasks to run.");
                        return true;
                    }

                    logger.LogDebug($"Handling NCS - Message Tasks : {string.Join(", ", taskNames)} - NCS Service Tasks found in Registry : {string.Join(", ", serviceTasks.Select(t => t.TaskName))}");

                    var ncsJobContextMessage = new NcsJobContextMessage(jobContextMessage);

                    var entryPoint = childLifetimeScope.Resolve<IEntryPoint>();
                    await entryPoint.CallbackAsync(tasks, ncsJobContextMessage, cancellationToken);
                }
                catch (OutOfMemoryException oom)
                {
                    Environment.FailFast("NCS Service Out of memory", oom);
                    throw;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                    throw;
                }

                logger.LogDebug($"Completed NCS Service for jobId:{jobContextMessage.JobId}");

                return true;
            }
        }

        public IEnumerable<string> GetTaskNamesForTopicFromMessage(JobContextMessage jobContextMessage)
        {
            return jobContextMessage
                .Topics[jobContextMessage.TopicPointer]
                .Tasks
                .SelectMany(t => t.Tasks);
        }
    }
}
