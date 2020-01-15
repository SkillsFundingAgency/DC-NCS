using ESFA.DC.NCS.Stateless.Config.Interfaces;

namespace ESFA.DC.NCS.Stateless.Config
{
    public class AzureStorageOptions : IAzureStorageOptions
    {
        public string DctAzureBlobConnectionString { get; set; }

        public string NcsAzureBlobConnectionString { get; set; }
    }
}
