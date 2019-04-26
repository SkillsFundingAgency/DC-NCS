using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.DataService;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.NCS.DataService
{
    public class NcsSubmissionQueryService : INcsSubmissionQueryService
    {
        private readonly Func<INcsContext> _ncsContext;

        public NcsSubmissionQueryService(Func<INcsContext> ncsContext)
        {
            _ncsContext = ncsContext;
        }

        public async Task<IEnumerable<NcsSubmission>> GetNcsSubmissionsAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (var context = _ncsContext())
            {
                return await context.NcsSubmissions
                    .Where(ns => ns.TouchpointId.Equals(ncsJobContextMessage.TouchpointId) && ns.CollectionYear.Equals(ncsJobContextMessage.CollectionYear))
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
