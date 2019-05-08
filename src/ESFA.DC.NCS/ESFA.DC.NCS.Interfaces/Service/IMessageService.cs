using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IMessageService
    {
        Task PublishNcsMessage(string status, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
