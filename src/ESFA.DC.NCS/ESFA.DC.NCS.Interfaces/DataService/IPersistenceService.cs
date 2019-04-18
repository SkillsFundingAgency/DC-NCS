using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.EF;

namespace ESFA.DC.NCS.Interfaces.DataService
{
    public interface IPersistenceService
    {
        Task PersistSubmissionAndFundingValuesAsync(IEnumerable<NcsSubmission> ncsSubmissions, IEnumerable<FundingValue> fundingValues, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);

        Task PersistSourceDataAsync(Source sourceData, CancellationToken cancellationToken);
    }
}
