using ESFA.DC.NCS.EF.Console.DesignTime.Naming;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace ESFA.DC.NCS.EF.Console.DesignTime
{
    public class DataDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPluralizer, DataPluralizer>();
            serviceCollection.AddSingleton<ICandidateNamingService, DataCandidateNamingService>();
        }
    }
}
