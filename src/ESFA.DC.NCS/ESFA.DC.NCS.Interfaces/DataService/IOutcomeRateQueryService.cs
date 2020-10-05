using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.EF;

namespace ESFA.DC.NCS.Interfaces.DataService
{
    public interface IOutcomeRateQueryService
    {
        Task<IEnumerable<OutcomeRate>> GetOutcomeRatesByPriorityAsync(int priorityGroup, CancellationToken cancellationToken);
    }
}
