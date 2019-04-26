using ESFA.DC.IO.AzureStorage.Config.Interfaces;

namespace ESFA.DC.NCS.Stateless.Config
{
    public class AzureStorageKeyValuePersistenceConfig : IAzureStorageKeyValuePersistenceServiceConfig
    {
        public AzureStorageKeyValuePersistenceConfig(string connectionString, string containerName)
        {
            ConnectionString = connectionString;
            ContainerName = containerName;
        }

        public string ConnectionString { get; }

        public string ContainerName { get; }
    }
}
