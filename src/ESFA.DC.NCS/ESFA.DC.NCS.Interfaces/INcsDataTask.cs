using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.NCS.Interfaces
{
    public interface INcsDataTask
    {
        string TaskName { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
