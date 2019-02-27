using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;

namespace ESFA.DC.NCS.Stateless
{
    public class JobContextMessageHandler : IMessageHandler<JobContextMessage>
    {
        public async Task<bool> HandleAsync(JobContextMessage message, CancellationToken cancellationToken)
        {
            Debug.WriteLine("Entered callback");
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
