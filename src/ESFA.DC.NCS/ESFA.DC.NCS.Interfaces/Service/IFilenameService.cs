using System;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IFilenameService
    {
        string GetFilename(int ukPrn, int jobId, string fileName, DateTime submissionDateTime, OutputTypes outputType);

        string GetZipName(int ukPrn, int jobId, string zipName);
    }
}
