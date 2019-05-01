namespace ESFA.DC.NCS.Stateless.Config.Interfaces
{
    public interface IDssServiceConfiguration
    {
        string DssDbConnectionString { get; }

        string DssQueueConnectionString { get; }

        string DssQueueName { get; }
    }
}
