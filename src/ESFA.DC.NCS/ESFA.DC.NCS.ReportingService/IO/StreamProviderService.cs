using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.NCS.Interfaces.IO;

namespace ESFA.DC.NCS.ReportingService.IO
{
    public class StreamProviderService : IStreamProviderService
    {
        public Stream GetStream(ZipArchive zipArchive, string fileName)
        {
            return zipArchive.CreateEntry(fileName, CompressionLevel.Optimal).Open();
        }
    }
}
