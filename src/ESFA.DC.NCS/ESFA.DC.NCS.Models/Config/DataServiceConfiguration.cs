using ESFA.DC.NCS.Models.Interfaces;

namespace ESFA.DC.NCS.Models.Config
{
    public class DataServiceConfiguration : IDataServiceConfiguration
    {
        public DataServiceConfiguration(string ncsDbConnection)
        {
            NcsDbConnection = ncsDbConnection;
        }

        public string NcsDbConnection { get; set; }
    }
}
