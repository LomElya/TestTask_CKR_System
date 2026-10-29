using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TestTask_CKR_System.Core.Networking.Interfaces;

namespace TestTask_CKR_System.Core.Networking
{
    public class RemoteImageService : IRemoteImageService
    {
        private readonly IHttpClient _httpClient;
        private readonly Dictionary<string, byte[]> _cache =
            new Dictionary<string, byte[]>(StringComparer.Ordinal);

        public RemoteImageService(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async UniTask<byte[]> GetAsync(
            string absoluteUrl,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(absoluteUrl))
                throw new ArgumentException("Image URL is required.", nameof(absoluteUrl));

            if (_cache.TryGetValue(absoluteUrl, out var cached))
                return cached;

            var data = await _httpClient.GetBytes(
                url: absoluteUrl,
                cancellationToken: cancellationToken);

            if (data == null || data.Length == 0)
                throw new FormatException("Image response contains no data.");

            _cache[absoluteUrl] = data;
            return data;
        }
    }
}