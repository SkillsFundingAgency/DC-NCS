using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.NCS.Models;
using ESFA.DC.NCS.Models.Interfaces;

namespace ESFA.DC.NCS.DataService
{
    public class DssDataRetrievalService : IDssDataRetrievalService
    {
        private readonly ILogger _logger;
        private readonly IDssConfig _dssConfig;

        public DssDataRetrievalService(ILogger logger, IDssConfig dssConfig)
        {
            _logger = logger;
            _dssConfig = dssConfig;
        }

        public async Task<IEnumerable<DssDataModel>> GetDataForTouchpoint(string touchpointId, DateTime dssSubmissionDateTime, DateTime fundingYearStart)
        {
            try
            {
                var parameters = GetParameters(touchpointId, fundingYearStart, dssSubmissionDateTime);

                using (IDbConnection db = new SqlConnection(_dssConfig.ConnectionString))
                {
                    return await db.QueryAsync<DssDataModel>(
                                @"SELECT 
	                                    [CustomerID],
	                                    [DateOfBirth],
	                                    [HomePostCode],
	                                    [ActionPlanId],
	                                    [SessionDate],
	                                    [SubContractorId],
	                                    [AdviserName],
	                                    [OutcomeId],
	                                    [OutcomeType],
	                                    [OutcomeEffectiveDate],
	                                    [OutcomePriorityCustomer] 
                                    FROM [dbo].[dcc-collections](@touchpointId, @startDate, @endDate)",
                                parameters,
                                commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to retrieve data for TouchpointId:{touchpointId}", ex);
                throw;
            }
        }

        private DynamicParameters GetParameters(string touchPointId, DateTime startDate, DateTime endDate)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@touchpointId", touchPointId);
            parameters.Add("@startDate", startDate);
            parameters.Add("@endDate", endDate);

            return parameters;
        }
    }
}
