using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        private readonly ILogger _logger;

        public FundingValueService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task PersistAsync(INcsContext ncsContext, IEnumerable<FundingValue> fundingValues, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            ncsContext.FundingValues.AddRange(fundingValues);
            await ncsContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteByTouchpointAsync(INcsContext ncsContext, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            SqlParameter touchPointId = new SqlParameter("@TouchpointId", ncsJobContextMessage.TouchpointId);
            string commandText = "DELETE FROM [NCS].[dbo].[FundingValues] WHERE [TouchpointId] = @TouchpointId";
            await ncsContext.Database.ExecuteSqlCommandAsync(commandText, touchPointId);
        }
    }
}
