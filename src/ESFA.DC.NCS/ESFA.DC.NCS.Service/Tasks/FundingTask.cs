using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Service.Helpers;

namespace ESFA.DC.NCS.Service.Tasks
{
    public class FundingTask : INcsDataTask
    {
        private readonly ILogger _logger;
        private readonly IMessageHelper _messageHelper;
        private readonly IDssDataRetrievalService _dssDataRetrievalService;
        private readonly IFundingService _fundingService;
        private readonly IPersistenceService _persistenceService;

        public FundingTask(ILogger logger, IMessageHelper messageHelper, IDssDataRetrievalService dssDataRetrievalService, IFundingService fundingService, IPersistenceService persistenceService)
        {
            _logger = logger;
            _messageHelper = messageHelper;
            _dssDataRetrievalService = dssDataRetrievalService;
            _fundingService = fundingService;
            _persistenceService = persistenceService;
        }

        public string TaskName => TaskNameConstants.FundingTaskName;

        public async Task ExecuteAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Debug.WriteLine("Entered Funding Task");

            var fundingYearStartDate = _messageHelper.CalculateFundingYearStart(ncsJobContextMessage.DssTimeStamp);
            var dssData = await _dssDataRetrievalService.GetDataForTouchpoint(ncsJobContextMessage.TouchpointId, ncsJobContextMessage.DssTimeStamp, fundingYearStartDate);

            if (dssData.Any())
            {
                _logger.LogInfo($"Retrieved {dssData.Count()} records from DSS for TouchpointId {ncsJobContextMessage.TouchpointId}");

                var ncsSubmission = ModelBuilder.BuildNcsSubmission(dssData, ncsJobContextMessage);
                var fundingValues = await _fundingService.CalculateFunding(ncsSubmission, ncsJobContextMessage, cancellationToken);

                await _persistenceService.PersistSubmissionAndFundingValuesAsync(ncsSubmission, fundingValues, ncsJobContextMessage, cancellationToken);
            }
            else
            {
                _logger.LogInfo($"0 records retrieved from DSS for TouchpointId {ncsJobContextMessage.TouchpointId}, exiting funding task");
            }
        }
    }
}
