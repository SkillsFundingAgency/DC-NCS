using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces.DataService;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.NCS.DataService
{
    public class OutcomeRateQueryService : IOutcomeRateQueryService
    {
        private readonly Func<INcsContext> _ncsContext;

        public OutcomeRateQueryService(Func<INcsContext> ncsContext)
        {
            _ncsContext = ncsContext;
        }

        public async Task<IEnumerable<OutcomeRate>> GetOutcomeRatesByPriorityAsync(int priorityGroup, CancellationToken cancellationToken)
        {
            List<OutcomeRate> outcomeRates;

            using (var context = _ncsContext())
            {
                outcomeRates = await context.OutcomeRates
                    .Where(or => or.OutcomePriorityCustomer == priorityGroup)
                    .ToListAsync(cancellationToken);
            }

            if (!outcomeRates.Any())
            {
                throw new Exception($"OutcomeRates table does not contain an outcome rate for the values: OutcomePriorityCustomer-{priorityGroup}");
            }

            return outcomeRates;
        }
    }
}
