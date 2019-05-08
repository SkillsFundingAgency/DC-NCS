using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.EF;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IFundingService
    {
        Task<IEnumerable<FundingValue>> CalculateFundingAsync(IEnumerable<NcsSubmission> ncsSubmissions, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
