namespace ESFA.DC.NCS.Stateless.Config.Interfaces
{
    public interface INcsServiceConfiguration
    {
        string DssServiceBusConnectionString { get; }

        string DssQueueName { get; }

        string TopicName { get; }

        string SubscriptionName { get; }

        string JobStatusQueueName { get; }

        string AuditQueueName { get; }
    }
}
