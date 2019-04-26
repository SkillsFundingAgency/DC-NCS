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
    public class FundingValueQueryService : IFundingValueQueryService
    {
        private readonly Func<INcsContext> _ncsContext;

        public FundingValueQueryService(Func<INcsContext> ncsContext)
        {
            _ncsContext = ncsContext;
        }

        public async Task<IEnumerable<FundingValue>> GetFundingValuesAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (var context = _ncsContext())
            {
                return await context.FundingValues
                    .Where(fv => fv.TouchpointId.Equals(ncsJobContextMessage.TouchpointId) && fv.CollectionYear.Equals(ncsJobContextMessage.CollectionYear))
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
