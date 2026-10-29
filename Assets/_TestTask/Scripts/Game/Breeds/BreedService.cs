using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TestTask_CKR_System.Core.Networking;
using TestTask_CKR_System.Core.Networking.Enum;
using TestTask_CKR_System.Core.Networking.Interfaces;
using TestTask_CKR_System.Game.Breeds.Config;

namespace TestTask_CKR_System.Game.Breeds
{
    public class BreedService : IBreedService
    {
        private const string ID = "id";
        private const string SIZE = "size";

        private readonly IHttpClient _httpClient;
        private readonly IBreedConfig _config;

        public BreedService(IHttpClient httpClient, IBreedConfig config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async UniTask<IReadOnlyList<BreedSummary>> GetBreedsAsync(
            CancellationToken cancellationToken)
        {
            var response = await _httpClient.Get<BreedListResponse>(
                EndpointType.BreedList,
                routeParams: GetSizeParamBreeds(),
                cancellationToken: cancellationToken);

            if (response.data == null || response.data.Length == 0)
                throw new FormatException("Breed list response contains no data.");

            var count = response.data.Length;
            var result = new List<BreedSummary>(count);
            for (var index = 0; index < count; index++)
            {
                var item = response.data[index];
                if (item.attributes.Equals(default(BreedAttributes)))
                    continue;

                result.Add(new BreedSummary(item.id, item.attributes.name));
            }

            return result;
        }

        public async UniTask<BreedDetails> GetBreedAsync(
            string breedId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(breedId))
                throw new ArgumentException("Breed id is required.", nameof(breedId));

            var response = await _httpClient.Get<BreedDetailsResponse>(
                EndpointType.BreedDetails,
                routeParams: GetRouteParamBreed(breedId),
                cancellationToken: cancellationToken);

            if (response.data.Equals(default(BreedResource)))
                throw new FormatException("Breed details response contains no data.");

            return new BreedDetails(
                response.data.id,
                response.data.attributes.name,
                response.data.attributes.description);
        }

        private Dictionary<string, string> GetSizeParamBreeds() =>
            _config.MaxListSize > 0
                ? new Dictionary<string, string> { [SIZE] = _config.MaxListSize.ToString() }
                : null;

        private Dictionary<string, string> GetRouteParamBreed(string id) =>
            new Dictionary<string, string> { [ID] = id };

        [Serializable]
        private struct BreedListResponse
        {
            public BreedResource[] data;
        }

        [Serializable]
        private struct BreedDetailsResponse
        {
            public BreedResource data;
        }

        [Serializable]
        private struct BreedResource
        {
            public string id;
            public BreedAttributes attributes;
        }

        [Serializable]
        private struct BreedAttributes
        {
            public string name;
            public string description;
        }
    }
}