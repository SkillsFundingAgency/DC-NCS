using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.CsvService;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Config;
using ESFA.DC.FileService.Interface;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.NCS.DataService;
using ESFA.DC.NCS.DataService.Org;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.NCS.Interfaces.IO;
using ESFA.DC.NCS.Interfaces.ReportingService;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models;
using ESFA.DC.NCS.Models.Config;
using ESFA.DC.NCS.Models.Interfaces;
using ESFA.DC.NCS.Models.Reports;
using ESFA.DC.NCS.ReportingService;
using ESFA.DC.NCS.ReportingService.IO;
using ESFA.DC.NCS.ReportingService.Mappers;
using ESFA.DC.NCS.ReportingService.Mappers.Factory;
using ESFA.DC.NCS.ReportingService.Reports;
using ESFA.DC.NCS.ReportingService.Reports.FundingSummary;
using ESFA.DC.NCS.Service;
using ESFA.DC.NCS.Service.Services;
using ESFA.DC.NCS.Service.Tasks;
using ESFA.DC.NCS.Stateless.Config;
using ESFA.DC.NCS.Stateless.Config.Interfaces;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Queueing.Interface.Configuration;
using ESFA.DC.ReferenceData.Organisations.Model;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.ServiceFabric.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.NCS.Stateless
{
    public static class DIComposition
    {
        public static ContainerBuilder BuildContainer()
        {
            var ncsConfiguration = GetNcsConfiguration();
            var dssConfiguration = GetDssConfiguration();
            var loggerConfiguration = GetLoggerConfiguration();
            var referenceDataConfiguration = GetReferenceDataConfiguration();
            var azureStorageOptions = GetAzureStorageOptions();

            return new ContainerBuilder()
                .RegisterLogger(loggerConfiguration)
                .RegisterSerializers()
                .RegisterJobContextManagementServices()
                .RegisterQueuesAndTopics(ncsConfiguration)
                .RegisterNcsService(ncsConfiguration)
                .RegisterDssService(dssConfiguration)
                .RegisterReports()
                .RegisterReferenceData(referenceDataConfiguration)
                .RegisterAzureStorage(azureStorageOptions);
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

        private static ContainerBuilder RegisterJobContextManagementServices(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<JobContextMessageHandler>().As<IMessageHandler<JobContextMessage>>();
            containerBuilder.RegisterType<JobContextManager<JobContextMessage>>().As<IJobContextManager<JobContextMessage>>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<DefaultJobContextMessageMapper<JobContextMessage>>().As<IMapper<JobContextMessage, JobContextMessage>>();
            containerBuilder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterQueuesAndTopics(this ContainerBuilder containerBuilder, INcsServiceConfiguration ncsServiceConfiguration)
        {
            var topicSubscriptionConfig = new TopicConfiguration(ncsServiceConfiguration.ServiceBusConnectionString, ncsServiceConfiguration.TopicName, ncsServiceConfiguration.SubscriptionName, 1, maximumCallbackTimeSpan: TimeSpan.FromMinutes(40));

            containerBuilder.Register(c => new TopicSubscriptionSevice<JobContextDto>(
                topicSubscriptionConfig,
                c.Resolve<IJsonSerializationService>(),
                c.Resolve<ILogger>())).As<ITopicSubscriptionService<JobContextDto>>();

            containerBuilder.Register(c =>
            {
                var topicPublishService =
                    new TopicPublishService<JobContextDto>(
                        topicSubscriptionConfig,
                        c.Resolve<IJsonSerializationService>());
                return topicPublishService;
            }).As<ITopicPublishService<JobContextDto>>();

            containerBuilder.Register(c =>
            {
                var auditPublishConfig = new QueueConfiguration(ncsServiceConfiguration.ServiceBusConnectionString, ncsServiceConfiguration.AuditQueueName, 1);

                return new QueuePublishService<AuditingDto>(
                    auditPublishConfig,
                    c.Resolve<IJsonSerializationService>());
            }).As<IQueuePublishService<AuditingDto>>();

            containerBuilder.Register(c =>
            {
                var jobStatusPublishConfig = new QueueConfiguration(ncsServiceConfiguration.ServiceBusConnectionString, ncsServiceConfiguration.JobStatusQueueName, 1);

                return new QueuePublishService<JobStatusDto>(
                    jobStatusPublishConfig,
                    c.Resolve<IJsonSerializationService>());
            }).As<IQueuePublishService<JobStatusDto>>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterNcsService(this ContainerBuilder containerBuilder, INcsServiceConfiguration ncsServiceConfiguration)
        {
            // Tasks
            containerBuilder.RegisterType<FundingTask>().As<INcsDataTask>();
            containerBuilder.RegisterType<ReportingTask>().As<INcsDataTask>();
            containerBuilder.RegisterType<EntryPoint>().As<IEntryPoint>();

            // Services
            containerBuilder.RegisterType<FundingService>().As<IFundingService>();
            containerBuilder.RegisterType<MessageService>().As<IMessageService>();
            containerBuilder.RegisterType<StreamProviderService>().As<IStreamProviderService>();
            containerBuilder.RegisterType<StorageService>().As<IStorageService>().WithAttributeFiltering();
            containerBuilder.RegisterType<ExcelService>().As<IExcelService>().WithAttributeFiltering();
            containerBuilder.RegisterType<FileNameService>().As<IFilenameService>();
            containerBuilder.RegisterType<ZipService>().As<IZipService>().WithAttributeFiltering();
            containerBuilder.RegisterType<OccupancyReportMapperFactory>().As<IClassMapFactory<OccupancyReportModel>>();
            containerBuilder.Register(c =>
            {
                var csvFileService = new CsvFileService(c.ResolveKeyed<IFileService>(PersistenceStorageKeys.DctAzureStorage));
                return csvFileService;
            }).As<ICsvFileService>();

            // Ncs database
            containerBuilder.RegisterType<NcsContext>().As<INcsContext>();
            containerBuilder.Register(container => new DbContextOptionsBuilder<NcsContext>()
                .UseSqlServer(ncsServiceConfiguration.NcsDbConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<NcsContext>>().SingleInstance();

            // Data services
            containerBuilder.RegisterType<PersistenceService>().As<IPersistenceService>();
            containerBuilder.RegisterType<OutcomeRateQueryService>().As<IOutcomeRateQueryService>();
            containerBuilder.RegisterType<NcsSubmissionQueryService>().As<INcsSubmissionQueryService>();
            containerBuilder.RegisterType<FundingValueQueryService>().As<IFundingValueQueryService>();
            containerBuilder.RegisterType<SourceQueryService>().As<ISourceQueryService>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterDssService(this ContainerBuilder containerBuilder, IDssServiceConfiguration dssServiceConfiguration)
        {
            containerBuilder.Register(c => new DssConfig(dssServiceConfiguration.DssDbConnectionString)).As<IDssConfig>().SingleInstance();

            containerBuilder.Register(c =>
            {
                var dssDataRetrievalService = new DssDataRetrievalService(
                    c.Resolve<ILogger>(),
                    c.Resolve<IDssConfig>());

                return dssDataRetrievalService;
            }).As<IDssDataRetrievalService>().InstancePerDependency();

            IQueueConfiguration dssPublishQueueConfiguration = new QueueConfiguration(
                dssServiceConfiguration.DssQueueConnectionString,
                dssServiceConfiguration.DssQueueName,
                1);

            containerBuilder.Register(c => new QueuePublishService<DssPublishMessageModel>(
                dssPublishQueueConfiguration,
                c.Resolve<IJsonSerializationService>())).As<IQueuePublishService<DssPublishMessageModel>>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterReports(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ReportingController>().As<IReportingController>().WithAttributeFiltering();
            containerBuilder.RegisterType<OccupancyReport>().As<IModelReport>();
            containerBuilder.RegisterType<FundingSummaryReport>().As<IModelReport>();
            containerBuilder.RegisterType<FundingSummaryReportBuilder>().As<IFundingSummaryReportBuilder>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterReferenceData(this ContainerBuilder containerBuilder, IReferenceDataConfiguration referenceDataConfiguration)
        {
            containerBuilder.RegisterType<OrganisationsContext>().As<IOrganisationsContext>();
            containerBuilder.Register(container => new DbContextOptionsBuilder<OrganisationsContext>()
                .UseSqlServer(referenceDataConfiguration.OrgDbConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<OrganisationsContext>>().SingleInstance();

            containerBuilder.RegisterType<OrgDataService>().As<IOrgDataService>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterAzureStorage(this ContainerBuilder containerBuilder, IAzureStorageOptions azureStorageOptions)
        {
            containerBuilder.Register(c =>
                new AzureStorageFileService(
                    new AzureStorageFileServiceConfiguration
                    {
                        ConnectionString = azureStorageOptions.DctAzureBlobConnectionString
                    }))
                .Keyed<IFileService>(PersistenceStorageKeys.DctAzureStorage)
                .SingleInstance();

            containerBuilder.Register(c =>
                    new AzureStorageFileService(
                        new AzureStorageFileServiceConfiguration
                        {
                            ConnectionString = azureStorageOptions.NcsAzureBlobConnectionString
                        }))
                .Keyed<IFileService>(PersistenceStorageKeys.DssAzureStorage)
                .SingleInstance();

            return containerBuilder;
        }

        private static INcsServiceConfiguration GetNcsConfiguration()
        {
            var configHelper = new ConfigurationHelper();

            return configHelper.GetSectionValues<NcsServiceConfiguration>("NcsServiceConfiguration");
        }

        private static IDssServiceConfiguration GetDssConfiguration()
        {
            var configHelper = new ConfigurationHelper();

            return configHelper.GetSectionValues<DssServiceConfiguration>("DssServiceConfiguration");
        }

        private static ILoggerConfiguration GetLoggerConfiguration()
        {
            var configHelper = new ConfigurationHelper();

            return configHelper.GetSectionValues<LoggerConfiguration>("LoggerConfiguration");
        }

        private static IReferenceDataConfiguration GetReferenceDataConfiguration()
        {
            var configHelper = new ConfigurationHelper();

            return configHelper.GetSectionValues<ReferenceDataConfiguration>("ReferenceDataConfiguration");
        }

        private static IAzureStorageOptions GetAzureStorageOptions()
        {
            var configHelper = new ConfigurationHelper();

            return configHelper.GetSectionValues<AzureStorageOptions>("AzureStorageOptions");
        }
    }
}
