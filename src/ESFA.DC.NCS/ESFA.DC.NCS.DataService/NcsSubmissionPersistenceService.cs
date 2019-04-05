using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.DataService;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.NCS.DataService
{
    public class NcsSubmissionPersistenceService : INcsSubmissionPersistenceService
    {
        public async Task PersistAsync(INcsContext ncsContext, IEnumerable<NcsSubmission> ncsSubmissions, CancellationToken cancellationToken)
        {
                ncsContext.NcsSubmissions.AddRange(ncsSubmissions);
                await ncsContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteByTouchpointAsync(INcsContext ncsContext, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            ncsContext.NcsSubmissions.RemoveRange(ncsContext.NcsSubmissions.Where(fv => fv.TouchpointId.Equals(ncsJobContextMessage.TouchpointId)));
            await ncsContext.SaveChangesAsync(cancellationToken);
        }
    }
}
