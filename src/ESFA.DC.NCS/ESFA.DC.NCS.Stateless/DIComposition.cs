using System.Collections.Generic;
using Autofac;
using ESFA.DC.CrossLoad.Dto;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Stateless.Config;
using ESFA.DC.NCS.Stateless.Config.Interfaces;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Queueing.Interface.Configuration;
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
            var loggerConfiguration = GetLoggerConfiguration();
            var databaseConfiguration = GetDatabaseConfiguration();

            return new ContainerBuilder()
                .RegisterLogger(loggerConfiguration)
                .RegisterSerializers()
                .RegisterMessageHandler()
                .RegisterQueues(ncsConfiguration);
        }

        private static ContainerBuilder RegisterLogger(this ContainerBuilder containerBuilder, ILoggerConfiguration loggerConfiguration)
        {
            containerBuilder.RegisterInstance(new LoggerConfiguration()
            {
                LoggerConnectionString = loggerConfiguration.LoggerConnectionString
            }).As<ILoggerConfiguration>().SingleInstance();

            containerBuilder.Register(c =>
            {
                var loggerOptions = c.Resolve<ILoggerConfiguration>();
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

        private static ContainerBuilder RegisterMessageHandler(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<MessageHandler>().As<IMessageHandler>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterQueues(this ContainerBuilder containerBuilder, INcsServiceConfiguration ncsServiceConfiguration)
        {
            IQueueConfiguration dssQueueConfiguration = new QueueConfiguration(
                ncsServiceConfiguration.DssServiceBusConnectionString,
                ncsServiceConfiguration.DssQueueName,
                1);

            containerBuilder.Register(c =>
            {
                return new QueueSubscriptionService<MessageCrossLoadDcftToDctDto>(
                    dssQueueConfiguration,
                    c.Resolve<IJsonSerializationService>(),
                    c.Resolve<ILogger>());
            }).As<IQueueSubscriptionService<MessageCrossLoadDcftToDctDto>>();

            return containerBuilder;
        }

        private static INcsServiceConfiguration GetNcsConfiguration()
        {
            var configHelper = new ConfigurationHelper();

            return configHelper.GetSectionValues<NcsServiceConfiguration>("NcsServiceConfiguration");
        }

        private static ILoggerConfiguration GetLoggerConfiguration()
        {
            var configHelper = new ConfigurationHelper();

            return configHelper.GetSectionValues<LoggerConfiguration>("LoggerConfiguration");
        }

        private static IDatabaseConfiguration GetDatabaseConfiguration()
        {
            var configHelper = new ConfigurationHelper();

            return configHelper.GetSectionValues<DatabaseConfiguration>("DatabaseConfiguration");
        }
    }
}
