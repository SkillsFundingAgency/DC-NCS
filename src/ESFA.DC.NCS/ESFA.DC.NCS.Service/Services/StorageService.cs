using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.FileService.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.NCS.Service.Services
{
    public class StorageService : IStorageService
    {
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IFileService _fileService;

        public StorageService(IJsonSerializationService jsonSerializationService, IFileService fileService)
        {
            _jsonSerializationService = jsonSerializationService;
            _fileService = fileService;
        }

        public async Task StoreAsJsonAsync<T>(T data, string fileName, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (var memoryStream = new MemoryStream())
            {
                _jsonSerializationService.Serialize(data, memoryStream);
            }

            using (var fileStream = await _fileService.OpenWriteStreamAsync(fileName, ncsJobContextMessage.DctContainer, cancellationToken))
            {
                _jsonSerializationService.Serialize(data, fileStream);
            }
        }
    }
}
