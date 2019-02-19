namespace ESFA.DC.NCS.Stateless.Config.Interfaces
{
    public interface INcsServiceConfiguration
    {
        string ServiceBusConnectionString { get; }

        string TopicName { get; }

        string SubscriptionName { get; }

        string JobStatusQueueName { get; }

        string AuditQueueName { get; }

        string LoggerConnectionString { get; }

        string DssDbConnectionString { get; }

        string NcsDbConnectionString { get; }
    }
}
