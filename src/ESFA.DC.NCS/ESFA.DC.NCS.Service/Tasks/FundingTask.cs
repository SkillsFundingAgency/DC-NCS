using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models;

namespace ESFA.DC.NCS.Service.Tasks
{
    public class FundingTask : INcsDataTask
    {
        private readonly ILogger _logger;
        private readonly IDssDataRetrievalService _dssDataRetrievalService;
        private readonly IFundingService _fundingService;
        private readonly IPersistenceService _persistenceService;
        private readonly IStorageService _storageService;
        private readonly IFilenameService _filenameService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FundingTask(
            ILogger logger,
            IDssDataRetrievalService dssDataRetrievalService,
            IFundingService fundingService,
            IPersistenceService persistenceService,
            IStorageService storageService,
            IFilenameService filenameService,
            IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _dssDataRetrievalService = dssDataRetrievalService;
            _fundingService = fundingService;
            _persistenceService = persistenceService;
            _storageService = storageService;
            _filenameService = filenameService;
            _dateTimeProvider = dateTimeProvider;
        }

        public string TaskName => TaskNameConstants.FundingTaskName;

        public async Task ExecuteAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Debug.WriteLine("Entered Funding Task");

            var fundingYearStartDate = CalculateFundingYearStart(ncsJobContextMessage);
            var dssData = await _dssDataRetrievalService.GetDataForTouchpoint(ncsJobContextMessage.TouchpointId, ncsJobContextMessage.DssTimeStamp, fundingYearStartDate);

            if (dssData.Any())
            {
                _logger.LogInfo($"Retrieved {dssData.Count()} records from DSS for TouchpointId {ncsJobContextMessage.TouchpointId}");

                var ncsSubmission = BuildNcsSubmission(dssData, ncsJobContextMessage);
                var fundingValues = await _fundingService.CalculateFundingAsync(ncsSubmission, ncsJobContextMessage, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                await _persistenceService.PersistSubmissionAndFundingValuesAsync(ncsSubmission, fundingValues, ncsJobContextMessage, cancellationToken);

                var fileName = _filenameService.GetFilename(ncsJobContextMessage.Ukprn, ncsJobContextMessage.JobId, FileNameConstants.sourceData, ncsJobContextMessage.DssTimeStamp, OutputTypes.Json);
                await _storageService.StoreAsJsonAsync(dssData, fileName, ncsJobContextMessage, cancellationToken);
            }
            else
            {
                _logger.LogInfo($"0 records retrieved from DSS for TouchpointId {ncsJobContextMessage.TouchpointId}, exiting funding task");
            }
        }

        private DateTime CalculateFundingYearStart(INcsJobContextMessage ncsJobContextMessage)
        {
            if (ncsJobContextMessage.CollectionYear == CollectionYearConstants.CollectionYear1920)
            {
                return CollectionYearConstants.FundingYear1920StartDate;
            }

            if (ncsJobContextMessage.CollectionYear == CollectionYearConstants.CollectionYear2021)
            {
                return CollectionYearConstants.FundingYear2021StartDate;
            }

            throw new Exception($"Collection year:{ncsJobContextMessage.CollectionYear} unknown.");
        }

        private ICollection<NcsSubmission> BuildNcsSubmission(IEnumerable<DssDataModel> dssData, INcsJobContextMessage ncsJobContextMessage)
        {
            return dssData.Select(item => new NcsSubmission
            {
                ActionPlanId = item.ActionPlanId,
                AdviserName = item.AdviserName,
                CustomerId = item.CustomerID,
                DateOfBirth = item.DateOfBirth,
                HomePostCode = item.HomePostCode,
                OutcomeEffectiveDate = item.OutcomeEffectiveDate,
                OutcomeId = item.OutcomeId,
                OutcomePriorityCustomer = item.OutcomePriorityCustomer,
                OutcomeType = item.OutcomeType,
                SubContractorId = item.SubContractorId,
                SessionDate = item.SessionDate,
                UKPRN = ncsJobContextMessage.Ukprn,
                TouchpointId = ncsJobContextMessage.TouchpointId,
                DssJobId = ncsJobContextMessage.DssJobId,
                DssTimestamp = ncsJobContextMessage.DssTimeStamp,
                CollectionYear = ncsJobContextMessage.CollectionYear,
                CreatedOn = _dateTimeProvider.GetNowUtc()
            }).ToList();
        }
    }
}
