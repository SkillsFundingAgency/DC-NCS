using ESFA.DC.NCS.Stateless.Config.Interfaces;

namespace ESFA.DC.NCS.Stateless.Config
{
    public class LoggerConfiguration : ILoggerConfiguration
    {
        public string LoggerConnectionString { get; set; }
    }
}
