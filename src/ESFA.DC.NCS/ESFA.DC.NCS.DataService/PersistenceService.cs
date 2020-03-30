using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.BulkCopy.Interfaces;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.NCS.Models.Interfaces;

namespace ESFA.DC.NCS.DataService
{
    public class PersistenceService : IPersistenceService
    {
        private readonly Func<INcsContext> _ncsContext;
        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBulkInsert _bulkInsert;
        private readonly IClearService _clearService;
        private readonly IDataServiceConfiguration _dataServiceConfiguration;

        public PersistenceService(Func<INcsContext> ncsContext, ILogger logger, IDateTimeProvider dateTimeProvider, IBulkInsert bulkInsert, IClearService clearService, IDataServiceConfiguration dataServiceConfiguration)
        {
            _ncsContext = ncsContext;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _bulkInsert = bulkInsert;
            _clearService = clearService;
            _dataServiceConfiguration = dataServiceConfiguration;
        }

        public async Task PersistSubmissionAndFundingValuesAsync(IEnumerable<NcsSubmission> ncsSubmissions, IEnumerable<FundingValue> fundingValues, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (SqlConnection ncsConnection = new SqlConnection(_dataServiceConfiguration.NcsDbConnection))
            {
                await ncsConnection.OpenAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                _logger.LogDebug("Starting NCS submission and funding data Transaction");

                using (SqlTransaction ncsTransaction = ncsConnection.BeginTransaction())
                {
                    try
                    {
                        await _clearService.ClearSubmissionDataAsync(ncsJobContextMessage, ncsConnection, ncsTransaction, cancellationToken);
                        await _bulkInsert.Insert(DatabaseConstants.NcsSubmissionTable, ncsSubmissions, ncsConnection, ncsTransaction, cancellationToken);

                        await _clearService.ClearFundingDataAsync(ncsJobContextMessage, ncsConnection, ncsTransaction, cancellationToken);
                        await _bulkInsert.Insert(DatabaseConstants.FundingValuesTable, fundingValues, ncsConnection, ncsTransaction, cancellationToken);

                        ncsTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug($"NCS Transaction failed rolling back - {ex.Message}");

                        ncsTransaction.Rollback();

                        throw;
                    }

                    _logger.LogDebug("NCS Transaction complete");
                }
            }
        }

        public async Task PersistSourceDataAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            var sourceData = new Source()
            {
                UKPRN = ncsJobContextMessage.Ukprn,
                TouchpointId = ncsJobContextMessage.TouchpointId,
                SubmissionDate = ncsJobContextMessage.DssTimeStamp,
                DssJobId = ncsJobContextMessage.DssJobId,
                CreatedOn = _dateTimeProvider.GetNowUtc()
            };

            using (var context = _ncsContext())
            {
                context.Sources.Add(sourceData);

                await context.SaveChangesAsync(cancellationToken);
                _logger.LogInfo("NCS Source data persisted Successfully.");
            }
        }
    }
}
