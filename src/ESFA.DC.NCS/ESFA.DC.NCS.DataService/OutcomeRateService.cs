using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces.DataService;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.NCS.DataService
{
    public class OutcomeRateService : IOutcomeRateService
    {
        private readonly ILogger _logger;
        private readonly Func<INcsContext> _ncsContext;

        public OutcomeRateService(ILogger logger, Func<INcsContext> ncsContext)
        {
            _logger = logger;
            _ncsContext = ncsContext;
        }

        public async Task<OutcomeRate> GetOutcomeRateByPriorityAndDeliveryAsync(int priorityGroup, string delivery, DateTime date, CancellationToken cancellationToken)
        {
            List<OutcomeRate> outcomeRates;

            using (var context = _ncsContext())
            {
                outcomeRates = await context.OutcomeRates
                    .Where(or => or.Delivery == delivery
                              && or.OutcomePriorityCustomer == priorityGroup
                              && date >= or.EffectiveFrom
                              && date < (or.EffectiveTo ?? DateTime.MaxValue))
                    .ToListAsync(cancellationToken);
            }

            if (!outcomeRates.Any())
            {
                throw new Exception($"OutcomeRates table does not contain an outcome rate for the values: OutcomePriorityCustomer-{priorityGroup}, Delivery-{delivery} and date-{date}");
            }

            if (outcomeRates.Count > 1)
            {
                throw new Exception($"OutcomeRates table contains more than one rate for the values: OutcomePriorityCustomer-{priorityGroup}, Delivery-{delivery} and date-{date}");
            }

            return outcomeRates.Single();
        }
    }
}
