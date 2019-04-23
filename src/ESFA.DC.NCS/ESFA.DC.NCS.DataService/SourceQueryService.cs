using System;
using System.Linq;
using System.Threading;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.DataService;

namespace ESFA.DC.NCS.DataService
{
    public class SourceQueryService : ISourceQueryService
    {
        private readonly Func<INcsContext> _ncsContext;
        private readonly ILogger _logger;

        public SourceQueryService(Func<INcsContext> ncsContext, ILogger logger)
        {
            _ncsContext = ncsContext;
            _logger = logger;
        }

        public DateTime? GetLastNcsSubmissionDate(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            DateTime? lastNcsSubmissionDate;

            cancellationToken.ThrowIfCancellationRequested();

            using (var context = _ncsContext())
            {
                lastNcsSubmissionDate = context.Sources
                    .Where(s => s.TouchpointId.Equals(ncsJobContextMessage.TouchpointId))
                    .OrderByDescending(s => s.SubmissionDate)
                    .FirstOrDefault()
                    ?.SubmissionDate;
            }

            return lastNcsSubmissionDate;
        }
    }
}
