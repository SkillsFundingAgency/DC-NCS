using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
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
        private readonly IDateTimeProvider _dateTimeProvider;

        public PersistenceService(Func<INcsContext> ncsContext, ILogger logger, IDateTimeProvider dateTimeProvider)
        {
            _ncsContext = ncsContext;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task PersistSubmissionAndFundingValuesAsync(IEnumerable<NcsSubmission> ncsSubmissions, IEnumerable<FundingValue> fundingValues, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (var context = _ncsContext())
            {
                context.NcsSubmissions.RemoveRange(context.NcsSubmissions.Where(ns => ns.TouchpointId.Equals(ncsJobContextMessage.TouchpointId) && ns.CollectionYear.Equals(ncsJobContextMessage.CollectionYear)));
                context.NcsSubmissions.AddRange(ncsSubmissions);

                context.FundingValues.RemoveRange(context.FundingValues.Where(fv => fv.TouchpointId.Equals(ncsJobContextMessage.TouchpointId) && fv.CollectionYear.Equals(ncsJobContextMessage.CollectionYear)));
                context.FundingValues.AddRange(fundingValues);

                await context.SaveChangesAsync(cancellationToken);
                _logger.LogInfo("NCS Submission and funding data persisted Successfully.");
            }
        }

        public async Task PersistSourceDataAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            var sourceData = new Source()
            {
                Ukprn = ncsJobContextMessage.Ukprn,
                TouchpointId = ncsJobContextMessage.TouchpointId,
                SubmissionDate = ncsJobContextMessage.DssTimeStamp,
                DssJobId = ncsJobContextMessage.DssJobId,
                CreatedOn = _dateTimeProvider.GetNowUtc()
            };

            using (var context = _ncsContext())
            {
                context.Sources.Add(sourceData);

                await context.SaveChangesAsync(cancellationToken);
                _logger.LogInfo("NCS Source data persisted Successfully.");
            }
        }
    }
}
