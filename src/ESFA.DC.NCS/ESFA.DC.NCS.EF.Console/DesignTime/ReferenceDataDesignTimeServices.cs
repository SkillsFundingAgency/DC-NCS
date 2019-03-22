﻿using ESFA.DC.ReferenceData.EF.Console.DesignTime;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace ESFA.DC.NCS.EF.Console.DesignTime
{
    public class ReferenceDataDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPluralizer, ReferenceDataPluralizer>();
        }
    }
}
