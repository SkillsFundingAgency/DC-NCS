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

namespace ESFA.DC.NCS.DataService
{
    public class PersistenceService : IPersistenceService
    {
        private readonly Func<INcsContext> _ncsContext;
        private readonly ILogger _logger;

        public PersistenceService(Func<INcsContext> ncsContext, ILogger logger)
        {
            _ncsContext = ncsContext;
            _logger = logger;
        }

        public async Task PersistSubmissionAndFundingValuesAsync(IEnumerable<NcsSubmission> ncsSubmissions, IEnumerable<FundingValue> fundingValues, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (var context = _ncsContext())
            {
                try
                {
                    context.NcsSubmissions.RemoveRange(context.NcsSubmissions.Where(fv => fv.TouchpointId.Equals(ncsJobContextMessage.TouchpointId)));
                    context.NcsSubmissions.AddRange(ncsSubmissions);

                    context.FundingValues.RemoveRange(context.FundingValues.Where(fv => fv.TouchpointId.Equals(ncsJobContextMessage.TouchpointId)));
                    context.FundingValues.AddRange(fundingValues);

                    await context.SaveChangesAsync(cancellationToken);
                    _logger.LogInfo("NCS Submission and funding data persisted Successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError("NCS Submission and funding data persist Failed", ex);
                    throw;
                }
            }
        }

        public async Task PersistSourceDataAsync(Source sourceData, CancellationToken cancellationToken)
        {
            using (var context = _ncsContext())
            {
                try
                {
                    context.Sources.Add(sourceData);

                    await context.SaveChangesAsync(cancellationToken);
                    _logger.LogInfo("NCS Source data persisted Successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError("NCS Source data persist Failed", ex);
                    throw;
                }
            }
        }
    }
}
