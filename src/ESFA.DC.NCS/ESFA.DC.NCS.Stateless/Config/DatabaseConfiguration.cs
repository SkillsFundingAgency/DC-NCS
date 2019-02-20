using ESFA.DC.NCS.Stateless.Config.Interfaces;

namespace ESFA.DC.NCS.Stateless.Config
{
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        public string DssDbConnectionString { get; set; }

        public string NcsDbConnectionString { get; set; }
    }
}
