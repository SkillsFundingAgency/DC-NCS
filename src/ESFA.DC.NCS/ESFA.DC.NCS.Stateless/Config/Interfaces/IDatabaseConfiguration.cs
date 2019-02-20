namespace ESFA.DC.NCS.Stateless.Config.Interfaces
{
    public interface IDatabaseConfiguration
    {
        string DssDbConnectionString { get; }

        string NcsDbConnectionString { get; }
    }
}
