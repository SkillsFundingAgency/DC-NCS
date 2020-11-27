using System;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.NCS.Interfaces.Service;

namespace ESFA.DC.NCS.Service
{
    public class FileNameService : IFilenameService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IDictionary<OutputTypes, string> _extensionsDictionary = new Dictionary<OutputTypes, string>()
        {
            [OutputTypes.Csv] = "csv",
            [OutputTypes.Excel] = "xlsx",
            [OutputTypes.Json] = "json",
            [OutputTypes.Zip] = "zip",
        };

        public FileNameService(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public string GetFilename(int ukPrn, int jobId, string fileName, DateTime submissionDateTime, OutputTypes outputType)
        {
            return $"{ukPrn}/{jobId}/{fileName}-{submissionDateTime:yyyyMMdd-HHmmss}.{GetExtension(outputType)}";
        }

        public string GetZipName(int ukPrn, int jobId, string zipName)
        {
            return $"{ukPrn}/{jobId}/{zipName}.{GetExtension(OutputTypes.Zip)}";
        }

        private string GetExtension(OutputTypes outputType) => _extensionsDictionary[outputType];
    }
}
