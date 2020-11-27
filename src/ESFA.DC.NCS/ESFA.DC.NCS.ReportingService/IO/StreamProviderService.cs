using System.IO;
using System.Linq;
using System.Reflection;
using ESFA.DC.NCS.Interfaces.IO;

namespace ESFA.DC.NCS.ReportingService.IO
{
    public class StreamProviderService : IStreamProviderService
    {
        public Stream GetStreamFromTemplate(string templateName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(templateName));
            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}
