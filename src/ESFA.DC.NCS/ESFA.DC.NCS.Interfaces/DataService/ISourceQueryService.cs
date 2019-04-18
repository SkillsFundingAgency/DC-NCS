using System;
using System.Threading;

namespace ESFA.DC.NCS.Interfaces.DataService
{
    public interface ISourceQueryService
    {
        DateTime? GetLastNcsSubmissionDate(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
