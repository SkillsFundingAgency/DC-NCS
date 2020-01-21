using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Stateless.Context;

namespace ESFA.DC.NCS.Stateless
{
    public class JobContextMessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<INcsDataTask> _ncsDataTasks;
        private readonly IEntryPoint _entryPoint;

        public JobContextMessageHandler(ILogger logger, IEnumerable<INcsDataTask> ncsDataTasks, IEntryPoint entryPoint)
        {
            _logger = logger;
            _ncsDataTasks = ncsDataTasks;
            _entryPoint = entryPoint;
        }

        public async Task<bool> HandleAsync(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            try
            {
                Debug.WriteLine("Entered message handler");
                _logger.LogDebug($"Started NCS Service for jobId:{jobContextMessage.JobId}");

                var taskNames = GetTaskNamesForTopicFromMessage(jobContextMessage);
                var tasks = _ncsDataTasks.Where(t => taskNames.Contains(t.TaskName)).ToList();

                if (!tasks.Any())
                {
                    _logger.LogInfo("NCS. No tasks to run.");
                    return true;
                }

                _logger.LogDebug($"Handling NCS - Message Tasks : {string.Join(", ", taskNames)} - NCS Service Tasks found in Registry : {string.Join(", ", _ncsDataTasks.Select(t => t.TaskName))}");

                var ncsJobContextMessage = new NcsJobContextMessage(jobContextMessage);
                await _entryPoint.CallbackAsync(tasks, ncsJobContextMessage, cancellationToken);
            }
            catch (OutOfMemoryException oom)
            {
                Environment.FailFast("NCS Service Out of memory", oom);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

            _logger.LogDebug($"Completed NCS Service for jobId:{jobContextMessage.JobId}");

            return true;
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
