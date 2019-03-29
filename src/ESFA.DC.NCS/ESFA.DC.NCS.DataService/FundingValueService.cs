using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
    public class FundingValueService : IFundingValueService
    {
        public async Task PersistAsync(INcsContext ncsContext, IEnumerable<FundingValue> fundingValues, CancellationToken cancellationToken)
        {
            ncsContext.FundingValues.AddRange(fundingValues);
            await ncsContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteByTouchpointAsync(INcsContext ncsContext, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            ncsContext.FundingValues.RemoveRange(ncsContext.FundingValues.Where(fv => fv.TouchpointId.Equals(ncsJobContextMessage.TouchpointId)));
            await ncsContext.SaveChangesAsync(cancellationToken);
        }
    }
}
