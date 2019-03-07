using ESFA.DC.NCS.Stateless.Config.Interfaces;

namespace ESFA.DC.NCS.Stateless.Config
{
    public class NcsServiceConfiguration : INcsServiceConfiguration
    {
        public string ServiceBusConnectionString { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public string JobStatusQueueName { get; set; }

        public string AuditQueueName { get; set; }

        public string NcsDbConnectionString { get; set; }
    }
}
