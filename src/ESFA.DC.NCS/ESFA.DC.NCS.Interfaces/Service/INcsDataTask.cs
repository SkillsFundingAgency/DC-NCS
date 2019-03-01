using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface INcsDataTask
    {
        string TaskName { get; }

        Task ExecuteAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
