using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Models;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IFundingService
    {
        Task<IEnumerable<FundingValue>> CalculateFunding(IEnumerable<NcsSubmission> ncsSubmissions, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
