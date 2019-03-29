using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.DataService;

namespace ESFA.DC.NCS.DataService
{
    public class PersistenceService : IPersistenceService
    {
        private readonly Func<INcsContext> _ncsContext;
        private readonly INcsSubmissionService _ncsSubmissionService;
        private readonly IFundingValueService _fundingValueService;
        private readonly ILogger _logger;

        public PersistenceService(Func<INcsContext> ncsContext, INcsSubmissionService ncsSubmissionService, IFundingValueService fundingValueService, ILogger logger)
        {
            _ncsContext = ncsContext;
            _ncsSubmissionService = ncsSubmissionService;
            _fundingValueService = fundingValueService;
            _logger = logger;
        }

        public async Task PersistSubmissionAndFundingValuesAsync(IEnumerable<NcsSubmission> ncsSubmissions, IEnumerable<FundingValue> fundingValues, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (var context = _ncsContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await _ncsSubmissionService.DeleteByTouchpointAsync(context, ncsJobContextMessage, cancellationToken);
                        await _ncsSubmissionService.PersistAsync(context, ncsSubmissions, cancellationToken);

                        await _fundingValueService.DeleteByTouchpointAsync(context, ncsJobContextMessage, cancellationToken);
                        await _fundingValueService.PersistAsync(context, fundingValues, cancellationToken);

                        transaction.Commit();

                        _logger.LogInfo("NCS Submission and funding data persisted Successfully.");
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError("NCS Submission and funding data persist Failed", exception);

                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
