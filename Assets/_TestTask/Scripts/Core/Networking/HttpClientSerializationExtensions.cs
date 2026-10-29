using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using TestTask_CKR_System.Core.Networking.Enum;
using TestTask_CKR_System.Core.Networking.Interfaces;

namespace TestTask_CKR_System.Core.Networking
{
    public static class HttpClientSerializationExtensions
    {
        public static async UniTask<TResponse> Get<TResponse>(
            this IHttpClient client,
            EndpointType endpoint,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
        {
            var text = await client.Get(endpoint, routeParams, queryParams, cancellationToken);
            return client.Deserialize<TResponse>(text);
        }

        public static UniTask<TResponse> Get<TRequest, TResponse>(
            this IHttpClient client,
            EndpointType endpoint,
            TRequest query,
            Dictionary<string, string> routeParams = null,
            CancellationToken cancellationToken = default)
        {
            return client.Get<TResponse>(
                endpoint,
                routeParams,
                ToParameters(query),
                cancellationToken);
        }

        public static async UniTask<TResponse> Post<TRequest, TResponse>(
            this IHttpClient client,
            EndpointType endpoint,
            TRequest body,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
        {
            var text = await client.Post(
                endpoint,
                client.JsonSerializer.Serialize(body),
                routeParams,
                queryParams,
                cancellationToken);
            return client.Deserialize<TResponse>(text);
        }

        public static async UniTask<TResponse> Patch<TRequest, TResponse>(
            this IHttpClient client,
            EndpointType endpoint,
            TRequest body,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
        {
            var text = await client.Patch(
                endpoint,
                client.JsonSerializer.Serialize(body),
                routeParams,
                queryParams,
                cancellationToken);
            return client.Deserialize<TResponse>(text);
        }

        public static async UniTask<TResponse> Delete<TResponse>(
            this IHttpClient client,
            EndpointType endpoint,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
        {
            var text = await client.Delete(
                endpoint,
                null,
                routeParams,
                queryParams,
                cancellationToken);
            return client.Deserialize<TResponse>(text);
        }

        public static async UniTask<TResponse> Delete<TRequest, TResponse>(
            this IHttpClient client,
            EndpointType endpoint,
            TRequest body,
            Dictionary<string, string> routeParams = null,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
        {
            var text = await client.Delete(
                endpoint,
                client.JsonSerializer.Serialize(body),
                routeParams,
                queryParams,
                cancellationToken);
            return client.Deserialize<TResponse>(text);
        }

        public static TResponse Deserialize<TResponse>(this IHttpClient client, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return default;

            try
            {
                return client.JsonSerializer.Deserialize<TResponse>(text);
            }
            catch (Exception exception)
            {
                throw new NetworkRequestException(
                    $"Response deserialization failed: {exception.Message}",
                    200);
            }
        }

        private static Dictionary<string, string> ToParameters<TRequest>(TRequest value)
        {
            if (ReferenceEquals(value, null))
                return null;

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var type = value.GetType();

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.CanRead)
                    AddParameter(result, property.Name, property.GetValue(value));
            }

            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                AddParameter(result, field.Name, field.GetValue(value));

            return result.Count == 0 ? null : result;
        }

        private static void AddParameter(
            IDictionary<string, string> parameters,
            string name,
            object value)
        {
            if (value == null)
                return;

            parameters[name] = value is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture)
                : value.ToString();
        }
    }
}