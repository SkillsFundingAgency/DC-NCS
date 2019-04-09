using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.NCS.Interfaces.IO
{
    public interface IStreamProviderService
    {
        Stream GetStream(ZipArchive zipArchive, string fileName);
    }
}
