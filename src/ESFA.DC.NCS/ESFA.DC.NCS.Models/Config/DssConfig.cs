using ESFA.DC.NCS.Models.Interfaces;

namespace ESFA.DC.NCS.Models.Config
{
    public class DssConfig : IDssConfig
    {
        public DssConfig(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }
    }
}
