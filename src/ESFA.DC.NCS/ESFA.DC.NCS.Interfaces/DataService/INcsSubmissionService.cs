using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;

namespace ESFA.DC.NCS.Interfaces.DataService
{
    public interface INcsSubmissionService
    {
        Task PersistAsync(INcsContext ncsContext, IEnumerable<NcsSubmission> ncsData, CancellationToken cancellationToken);

        Task DeleteByTouchpointAsync(INcsContext ncsContext, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
