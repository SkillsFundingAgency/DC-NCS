using System.IO;

namespace ESFA.DC.NCS.Interfaces.IO
{
    public interface IStreamProviderService
    {
        Stream GetStreamFromTemplate(string templateName);
    }
}
