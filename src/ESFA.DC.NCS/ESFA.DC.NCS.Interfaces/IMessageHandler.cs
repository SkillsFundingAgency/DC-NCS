using ESFA.DC.CrossLoad.Dto;
using ESFA.DC.Queueing.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.NCS.Interfaces
{
    public interface IMessageHandler
    {
        Task<IQueueCallbackResult> Callback(MessageCrossLoadDcftToDctDto message, IDictionary<string, object> messageProperties, CancellationToken cancellationToken);
    }
}
