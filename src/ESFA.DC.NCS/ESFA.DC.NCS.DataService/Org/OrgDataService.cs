using System;
using System.Linq;
using System.Threading;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;

namespace ESFA.DC.NCS.DataService.Org
{
    public class OrgDataService : IOrgDataService
    {
        private readonly Func<IOrganisationsContext> _orgContext;

        public OrgDataService(Func<IOrganisationsContext> orgContext)
        {
            _orgContext = orgContext;
        }

        public string GetProviderName(int ukPrn, CancellationToken cancellationToken)
        {
            string providerName;

            cancellationToken.ThrowIfCancellationRequested();

            using (var context = _orgContext())
            {
                providerName = context.OrgDetails
                    .FirstOrDefault(o => o.Ukprn == ukPrn)
                    ?.Name;
            }

            return providerName;
        }
    }
}
