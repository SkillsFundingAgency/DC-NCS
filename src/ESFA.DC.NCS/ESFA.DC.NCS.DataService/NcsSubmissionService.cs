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
        private readonly Func<INcsContext> _ncsContext;

        public NcsSubmissionService(ILogger logger, Func<INcsContext> ncsContext)
        {
            _logger = logger;
            _ncsContext = ncsContext;
        }

        public async Task PersistAsync(IEnumerable<NcsSubmission> ncsSubmissions, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (var context = _ncsContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Clean up TouchPoint Data
                        SqlParameter touchPointId = new SqlParameter("@TouchpointId", ncsJobContextMessage.TouchpointId);
                        string commandText = "DELETE FROM [NCS].[dbo].[NcsSubmission] WHERE [TouchpointId] = @TouchpointId";
                        await context.Database.ExecuteSqlCommandAsync(commandText, touchPointId);

                        // Insert new values
                        foreach (var submission in ncsSubmissions)
                        {
                            context.NcsSubmissions.AddRange(submission);
                        }

                        await context.SaveChangesAsync(cancellationToken);
                        transaction.Commit();

                        _logger.LogInfo("NCS Submission Persisted Successfully.");
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError("NCS Submission Persist Failed", exception);

                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
