using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IEntryPoint
    {
        Task<bool> CallbackAsync(IEnumerable<INcsDataTask> ncsTasks, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
