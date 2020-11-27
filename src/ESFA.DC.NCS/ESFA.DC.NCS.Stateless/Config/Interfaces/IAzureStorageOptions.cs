namespace ESFA.DC.NCS.Stateless.Config.Interfaces
{
    public interface IAzureStorageOptions
    {
        string DctAzureBlobConnectionString { get; }

        string NcsAzureBlobConnectionString { get; }
    }
}
