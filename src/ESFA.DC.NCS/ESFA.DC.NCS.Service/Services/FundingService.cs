using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models;

namespace ESFA.DC.NCS.Service.Services
{
    public class FundingService : IFundingService
    {
        private readonly ILogger _logger;

        public FundingService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<NcsSubmission>> CalculateFunding(IEnumerable<NcsSubmission> ncsSubmissions)
        {
            throw new NotImplementedException();
        }
    }
}
