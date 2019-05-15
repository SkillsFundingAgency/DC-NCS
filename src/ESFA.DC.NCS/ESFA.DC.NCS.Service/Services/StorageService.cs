using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.NCS.Service.Services
{
    public class StorageService : IStorageService
    {
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IStreamableKeyValuePersistenceService _storage;

        public StorageService(
            IJsonSerializationService jsonSerializationService,
            [KeyFilter(PersistenceStorageKeys.DctAzureStorage)] IStreamableKeyValuePersistenceService storage)
        {
            _jsonSerializationService = jsonSerializationService;
            _storage = storage;
        }

        public async Task StoreAsJsonAsync<T>(T data, string fileName, INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken)
        {
            using (var memoryStream = new MemoryStream())
            {
                _jsonSerializationService.Serialize(data, memoryStream);

                await _storage.SaveAsync($"{ncsJobContextMessage.Ukprn}_{ncsJobContextMessage.JobId}_{fileName}.json", memoryStream, cancellationToken);
            }
        }
    }
}
