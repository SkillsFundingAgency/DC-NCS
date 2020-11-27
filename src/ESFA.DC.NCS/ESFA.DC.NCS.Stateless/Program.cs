using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Autofac.Integration.ServiceFabric;
using ESFA.DC.NCS.Stateless.Config;
using ESFA.DC.ServiceFabric.Helpers;
using ESFA.DC.ServiceFabric.Helpers.Interfaces;

namespace ESFA.DC.NCS.Stateless
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                IConfigurationHelper configHelper = new ConfigurationHelper();

                // License Aspose.Cells
                SoftwareLicenceSection softwareLicenceSection = configHelper.GetSectionValues<SoftwareLicenceSection>(nameof(SoftwareLicenceSection));
                if (!string.IsNullOrEmpty(softwareLicenceSection.AsposeLicence))
                {
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(softwareLicenceSection.AsposeLicence.Replace("&lt;", "<").Replace("&gt;", ">"))))
                    {
                        new Aspose.Cells.License().SetLicense(ms);
                    }
                }

                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                var builder = DIComposition.BuildContainer();

                builder.RegisterServiceFabricSupport();

                builder.RegisterStatelessService<Stateless>("ESFA.DC.NCS.StatelessType");

                using (var container = builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Stateless).Name);

                    // Prevents this host process from terminating so services keep running.
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
