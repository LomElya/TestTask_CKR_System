using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TestTask_CKR_System.Core.Networking.Enum;
using TestTask_CKR_System.Core.Utils.Serializer;

namespace TestTask_CKR_System.Core.Networking.Interfaces
{
    public interface IHttpClient
    {
        IJsonSerializer JsonSerializer { get; }

        UniTask<string> Get(
           EndpointType endpoint,
           Dictionary<string, string> routeParams = null,
           Dictionary<string, string> queryParams = null,
           CancellationToken cancellationToken = default);

        UniTask<string> Post(
            EndpointType endpoint,
            string body = null,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default);

        UniTask<string> Patch(
            EndpointType endpoint,
            string body = null,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default);

        UniTask<string> Delete(
            EndpointType endpoint,
            string body = null,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default);

        UniTask<byte[]> GetBytes(string url, CancellationToken cancellationToken = default);
    }
}