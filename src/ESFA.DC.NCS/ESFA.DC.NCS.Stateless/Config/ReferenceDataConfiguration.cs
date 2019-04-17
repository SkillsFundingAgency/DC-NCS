using ESFA.DC.NCS.Stateless.Config.Interfaces;

namespace ESFA.DC.NCS.Stateless.Config
{
    public class ReferenceDataConfiguration : IReferenceDataConfiguration
    {
        public string OrgDbConnectionString { get; set; }
    }
}
