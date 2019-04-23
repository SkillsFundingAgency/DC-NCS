using System.Threading;

namespace ESFA.DC.NCS.Interfaces.DataService
{
    public interface IOrgDataService
    {
        string GetProviderName(int ukPrn, CancellationToken cancellationToken);
    }
}
