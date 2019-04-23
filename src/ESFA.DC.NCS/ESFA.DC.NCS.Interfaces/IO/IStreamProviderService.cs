using System.IO;
using System.IO.Compression;

namespace ESFA.DC.NCS.Interfaces.IO
{
    public interface IStreamProviderService
    {
        Stream GetStream(ZipArchive zipArchive, string fileName);

        Stream GetStreamFromTemplate(string templateName);
    }
}
