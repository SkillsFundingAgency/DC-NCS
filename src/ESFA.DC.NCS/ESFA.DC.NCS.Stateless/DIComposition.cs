using System.Collections.Generic;
using Autofac;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Stateless.Config;
using ESFA.DC.NCS.Stateless.Config.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.ServiceFabric.Helpers;

namespace ESFA.DC.NCS.Stateless
{
    public static class DIComposition
    {
        public static ContainerBuilder BuildContainer()
        {
            var ncsConfiguration = GetNcsConfiguration();

            return new ContainerBuilder()
                .RegisterLogger(ncsConfiguration)
                .RegisterSerializers();
        }

        private static ContainerBuilder RegisterLogger(this ContainerBuilder containerBuilder, INcsServiceConfiguration ncsServiceConfiguration)
        {
            containerBuilder.RegisterInstance(new LoggerOptions()
            {
                LoggerConnectionString = ncsServiceConfiguration.LoggerConnectionString
            }).As<ILoggerOptions>().SingleInstance();

            containerBuilder.Register(c =>
            {
                var loggerOptions = c.Resolve<ILoggerOptions>();
                return new ApplicationLoggerSettings
                {
                    ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>()
                    {
                        new MsSqlServerApplicationLoggerOutputSettings()
                        {
                            MinimumLogLevel = LogLevel.Verbose,
                            ConnectionString = loggerOptions.LoggerConnectionString
                        },
                        new ConsoleApplicationLoggerOutputSettings()
                        {
                            MinimumLogLevel = LogLevel.Verbose
                        }
                    }
                };
            }).As<IApplicationLoggerSettings>().SingleInstance();

            containerBuilder.RegisterType<Logging.ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SeriLogger>().As<ILogger>().InstancePerLifetimeScope();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterSerializers(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();

            return containerBuilder;
        }

        private static INcsServiceConfiguration GetNcsConfiguration()
        {
            var configHelper = new ConfigurationHelper();

            return configHelper.GetSectionValues<NcsServiceConfiguration>("NcsServiceConfiguration");
        }
    }
}
