using ESFA.DC.NCS.Stateless.Config.Interfaces;

namespace ESFA.DC.NCS.Stateless.Config
{
    public class DssServiceConfiguration : IDssServiceConfiguration
    {
        public string DssDbConnectionString { get; set; }
    }
}
