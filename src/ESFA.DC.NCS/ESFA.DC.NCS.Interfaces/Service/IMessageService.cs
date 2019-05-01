using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IMessageService
    {
        Task PublishNcsSuccessMessage(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
