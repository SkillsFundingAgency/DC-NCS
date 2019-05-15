using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IStorageService
    {
        Task StoreAsJsonAsync<T>(T data, string fileName, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
