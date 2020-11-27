namespace ESFA.DC.NCS.Stateless.Config.Interfaces
{
    public interface INcsServiceConfiguration
    {
        string ServiceBusConnectionString { get; }

        string TopicName { get; }

        string SubscriptionName { get; }

        string JobStatusQueueName { get; }

        string AuditQueueName { get; }

        string NcsDbConnectionString { get; }
    }
}
