using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TestTask_CKR_System.Core.Networking.Config;
using TestTask_CKR_System.Core.Networking.Enum;
using TestTask_CKR_System.Core.Networking.Interfaces;
using TestTask_CKR_System.Core.Utils.Serializer;
using UnityEngine;
using UnityEngine.Networking;

namespace TestTask_CKR_System.Core.Networking
{
    public class UnityWebRequestHttpClient : IHttpClient
    {
        private readonly NetworkConfig _config;
        public IJsonSerializer JsonSerializer { get; }

        public UnityWebRequestHttpClient(
            NetworkConfig config,
            IJsonSerializer jsonSerializer)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            JsonSerializer = jsonSerializer ??
                throw new ArgumentNullException(nameof(jsonSerializer));
        }


        public async UniTask<string> Get(
            EndpointType endpoint,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
        {
            var response = await Send(
                HttpVerb.GET,
                endpoint,
                null,
                routeParams,
                queryParams,
                cancellationToken);
            return response.Text;
        }

        public async UniTask<string> Post(
            EndpointType endpoint,
            string body = null,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
        {
            var response = await Send(
                HttpVerb.POST,
                endpoint,
                body,
                routeParams,
                queryParams,
                cancellationToken);
            return response.Text;
        }

        public async UniTask<string> Patch(
            EndpointType endpoint,
            string body = null,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
        {
            var response = await Send(
                HttpVerb.PATCH,
                endpoint,
                body,
                routeParams,
                queryParams,
                cancellationToken);
            return response.Text;
        }

        public async UniTask<string> Delete(
            EndpointType endpoint,
            string body = null,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
        {
            var response = await Send(
                HttpVerb.DELETE,
                endpoint,
                body,
                routeParams,
                queryParams,
                cancellationToken);
            return response.Text;
        }

        public async UniTask<byte[]> GetBytes(string url, CancellationToken cancellationToken = default)
        {
            var response = await Send(
                HttpVerb.GET,
                url,
                cancellationToken);
            return response.Data;
        }

        private async UniTask<HttpResponse> Send(
            HttpVerb verb,
            EndpointType endpoint,
            string body,
            Dictionary<string, string> routeParams,
            Dictionary<string, string> queryParams,
            CancellationToken cancellationToken)
        {
            var definition = _config.GetEndpoint(endpoint);
            if (definition.Verb != verb)
            {
                throw new InvalidOperationException(
                    $"Endpoint '{endpoint}' is configured as {definition.Verb}, " +
                    $"but was called as {verb}.");
            }

            var url = ApplyRouteParams(definition.Url, routeParams);
            url = AppendQuery(url, queryParams);

            Debug.Log($"[Network][SEND RQUEST] Url: {url}");

            using var request = BuildRequest(verb, url, body, definition.ContentType);
            request.timeout = _config.RequestTimeoutSeconds;
            ApplyHeaders(request, definition.Headers);

            using var cancellationRegistration =
                cancellationToken.Register(() => request.Abort());

            try
            {
#if UNITY_2022_2_OR_NEWER
                await request.SendWebRequest().WithCancellation(cancellationToken);
#else
                await request.SendWebRequest();
                cancellationToken.ThrowIfCancellationRequested();
#endif
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new NetworkRequestException(
                    exception.Message,
                    request.responseCode);
            }

#if UNITY_2020_2_OR_NEWER
            var failed = request.result == UnityWebRequest.Result.ConnectionError ||
                         request.result == UnityWebRequest.Result.ProtocolError ||
                         request.result == UnityWebRequest.Result.DataProcessingError;
#else
            var failed = request.isNetworkError || request.isHttpError;
#endif
            if (failed)
                throw new NetworkRequestException(request.error, request.responseCode);

            Debug.Log($"[Network][SEND RESPONSE] Url: {request.downloadHandler?.text}");

            return new HttpResponse(
                request.responseCode,
                request.downloadHandler?.data);
        }

        public async UniTask<HttpResponse> Send(
            HttpVerb verb,
            string url,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(url))
                throw new NetworkRequestException("Url is Empty", 400);

            Debug.Log($"[Network][SEND RQUEST] Url: {url}");

            using var request = BuildRequest(verb, url, null);
            request.timeout = _config.RequestTimeoutSeconds;

            using var cancellationRegistration =
                cancellationToken.Register(() => request.Abort());

            try
            {
#if UNITY_2022_2_OR_NEWER
                await request.SendWebRequest().WithCancellation(cancellationToken);
#else
                await request.SendWebRequest();
                cancellationToken.ThrowIfCancellationRequested();
#endif
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new NetworkRequestException(
                    exception.Message,
                    request.responseCode);
            }

#if UNITY_2020_2_OR_NEWER
            var failed = request.result == UnityWebRequest.Result.ConnectionError ||
                         request.result == UnityWebRequest.Result.ProtocolError ||
                         request.result == UnityWebRequest.Result.DataProcessingError;
#else
            var failed = request.isNetworkError || request.isHttpError;
#endif
            if (failed)
                throw new NetworkRequestException(request.error, request.responseCode);

            Debug.Log($"[Network][SEND RESPONSE] Url: {request.downloadHandler?.text}");

            return new HttpResponse(
                request.responseCode,
                request.downloadHandler?.data);
        }

        private UnityWebRequest BuildRequest(
            HttpVerb verb,
            string url,
            string body,
            string contentType = null)
        {
            UnityWebRequest request = GetRequest(verb, url, body);

            if (request.downloadHandler == null)
                request.downloadHandler = new DownloadHandlerBuffer();

            if (request.uploadHandler != null)
            {
                request.SetRequestHeader(
                    "Content-Type",
                    string.IsNullOrWhiteSpace(contentType)
                        ? "application/json"
                        : contentType);
            }

            return request;
        }

        private UnityWebRequest GetRequest(
            HttpVerb verb, string url, string body = null) =>
            verb switch
            {
                HttpVerb.GET => UnityWebRequest.Get(url),
                HttpVerb.POST => CreateBodyRequest(UnityWebRequest.kHttpVerbPOST, url, body),
                HttpVerb.PATCH => CreateBodyRequest("PATCH", url, body),
                HttpVerb.DELETE => string.IsNullOrEmpty(body)
                                        ? UnityWebRequest.Delete(url)
                                        : CreateBodyRequest(UnityWebRequest.kHttpVerbDELETE, url, body),
                _ => throw new ArgumentOutOfRangeException(nameof(verb), verb, null)
            };


        private static UnityWebRequest CreateBodyRequest(
            string method,
            string url,
            string body)
        {
            return new UnityWebRequest(url, method)
            {
                uploadHandler = new UploadHandlerRaw(
                    Encoding.UTF8.GetBytes(body ?? string.Empty)),
                downloadHandler = new DownloadHandlerBuffer()
            };
        }

        private void ApplyHeaders(UnityWebRequest request, HeaderPair[] endpointHeaders)
        {
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            MergeHeaders(headers, _config.DefaultHeaders);
            MergeHeaders(headers, endpointHeaders);

            foreach (var header in headers)
                request.SetRequestHeader(header.Key, header.Value);
        }

        private static void MergeHeaders(
            IDictionary<string, string> target,
            HeaderPair[] source)
        {
            if (source == null)
                return;

            foreach (var header in source)
            {
                if (!string.IsNullOrWhiteSpace(header.Key))
                    target[header.Key] = header.Value ?? string.Empty;
            }
        }

        private static string ApplyRouteParams(
            string template,
            Dictionary<string, string> routeParams)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new InvalidOperationException("Endpoint URL is empty.");

            if (template == "{url}")
            {
                if (routeParams == null ||
                    !routeParams.TryGetValue("url", out var absoluteUrl) ||
                    !Uri.TryCreate(absoluteUrl, UriKind.Absolute, out var uri) ||
                    !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(
                        "The '{url}' endpoint requires an absolute HTTPS route parameter.",
                        nameof(routeParams));
                }

                return absoluteUrl;
            }

            var result = template;
            if (routeParams != null)
            {
                foreach (var parameter in routeParams)
                {
                    if (string.IsNullOrWhiteSpace(parameter.Key))
                        continue;

                    result = result.Replace(
                        $"{{{parameter.Key}}}",
                        UnityWebRequest.EscapeURL(parameter.Value ?? string.Empty));
                }
            }

            if (result.Contains("{") || result.Contains("}"))
                throw new ArgumentException($"Not all route parameters were supplied for '{template}'.");

            return result;
        }

        private static string AppendQuery(
            string url,
            Dictionary<string, string> queryParams)
        {
            if (queryParams == null || queryParams.Count == 0)
                return url;

            var builder = new StringBuilder(url);
            var hasParameters = url.Contains("?");

            foreach (var parameter in queryParams)
            {
                if (string.IsNullOrWhiteSpace(parameter.Key) || parameter.Value == null)
                    continue;

                builder.Append(hasParameters ? '&' : '?');
                builder.Append(UnityWebRequest.EscapeURL(parameter.Key));
                builder.Append('=');
                builder.Append(UnityWebRequest.EscapeURL(parameter.Value));
                hasParameters = true;
            }

            return builder.ToString();
        }
    }

    public sealed class HttpResponse
    {
        public HttpResponse(long statusCode, byte[] data)
        {
            StatusCode = statusCode;
            Data = data ?? Array.Empty<byte>();
        }

        public long StatusCode { get; }
        public byte[] Data { get; }
        public string Text => Encoding.UTF8.GetString(Data);
    }
}