using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.DataService;

namespace ESFA.DC.NCS.DataService
{
    public class ClearService : IClearService
    {
        public async Task ClearSubmissionDataAsync(INcsJobContextMessage ncsJobContextMessage, SqlConnection sqlConnection, SqlTransaction sqlTransaction, CancellationToken cancellationToken)
        {
            using (SqlCommand sqlCommand = new SqlCommand(BuildDeleteSubmissionDataSql(ncsJobContextMessage), sqlConnection, sqlTransaction))
            {
                await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public async Task ClearFundingDataAsync(INcsJobContextMessage ncsJobContextMessage, SqlConnection sqlConnection, SqlTransaction sqlTransaction, CancellationToken cancellationToken)
        {
            using (SqlCommand sqlCommand = new SqlCommand(BuildDeleteFundingDataSql(ncsJobContextMessage), sqlConnection, sqlTransaction))
            {
                await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        private string BuildDeleteSubmissionDataSql(INcsJobContextMessage ncsJobContextMessage)
        {
            return $@"DELETE FROM {DatabaseConstants.NcsSubmissionTable} 
                      WHERE TouchpointId = '{ncsJobContextMessage.TouchpointId}' 
                      AND CollectionYear = {ncsJobContextMessage.CollectionYear}";
        }

        private string BuildDeleteFundingDataSql(INcsJobContextMessage ncsJobContextMessage)
        {
            return $@"DELETE FROM {DatabaseConstants.FundingValuesTable}
                      WHERE TouchpointId = '{ncsJobContextMessage.TouchpointId}' 
                      AND CollectionYear = {ncsJobContextMessage.CollectionYear}";
        }
    }
}
