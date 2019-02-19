using ESFA.DC.NCS.Stateless.Config.Interfaces;

namespace ESFA.DC.NCS.Stateless.Config
{
    public class LoggerOptions : ILoggerOptions
    {
        public string LoggerConnectionString { get; set; }
    }
}
