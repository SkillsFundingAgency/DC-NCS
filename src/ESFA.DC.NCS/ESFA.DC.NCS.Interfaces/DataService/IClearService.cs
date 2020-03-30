using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.NCS.Interfaces.DataService
{
    public interface IClearService
    {
        Task ClearSubmissionDataAsync(INcsJobContextMessage ncsJobContextMessage, SqlConnection sqlConnection, SqlTransaction sqlTransaction, CancellationToken cancellationToken);

        Task ClearFundingDataAsync(INcsJobContextMessage ncsJobContextMessage, SqlConnection sqlConnection, SqlTransaction sqlTransaction, CancellationToken cancellationToken);
    }
}
