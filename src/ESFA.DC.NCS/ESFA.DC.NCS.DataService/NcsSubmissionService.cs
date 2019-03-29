using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class NcsSubmissionService : INcsSubmissionService
    {
        private readonly ILogger _logger;

        public NcsSubmissionService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task PersistAsync(INcsContext ncsContext, IEnumerable<NcsSubmission> ncsSubmissions, CancellationToken cancellationToken)
        {
                ncsContext.NcsSubmissions.AddRange(ncsSubmissions);
                await ncsContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteByTouchpointAsync(INcsContext ncsContext, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            SqlParameter touchPointId = new SqlParameter("@TouchpointId", ncsJobContextMessage.TouchpointId);
            string commandText = "DELETE FROM [NCS].[dbo].[NcsSubmission] WHERE [TouchpointId] = @TouchpointId";
            await ncsContext.Database.ExecuteSqlCommandAsync(commandText, touchPointId);
        }
    }
}
