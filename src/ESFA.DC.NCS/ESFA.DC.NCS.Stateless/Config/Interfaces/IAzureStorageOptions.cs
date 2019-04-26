namespace ESFA.DC.NCS.Stateless.Config.Interfaces
{
    public interface IAzureStorageOptions
    {
        string DctAzureBlobConnectionString { get; }

        string DctAzureBlobContainerName { get; }

        string NcsAzureBlobConnectionString { get; }

        string NcsAzureBlobContainerName { get; }
    }
}
