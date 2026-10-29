using TestTask_CKR_System.Core.Networking.Enum;
using UnityEngine;

namespace TestTask_CKR_System.Core.Networking.Config
{
    [CreateAssetMenu(fileName = "NetworkConfig", menuName = "TestTask CKR-System/NetworkConfig")]
    public  class NetworkConfig : ScriptableObject
    {
        [Header("Request policy")]
        // [SerializeField, Min(0.1f)] private float _weatherPollingIntervalSeconds = 5f;
        // [SerializeField, Range(1, 1000)] private int _breedListLimit = 10;
        [SerializeField, Min(1)] private int _requestTimeoutSeconds = 20;

        [SerializeField] private EndpointDef[] _endpoints;

        [Space(10)]
        [SerializeField] private HeaderPair[] _defaultHeaders;

        public int RequestTimeoutSeconds => _requestTimeoutSeconds;
        public HeaderPair[] DefaultHeaders => _defaultHeaders;

        public EndpointDef GetEndpoint(EndpointType endpoint)
        {
            foreach (var data in _endpoints)
            {
                if (data.Endpoint == endpoint)
                    return data;
            }

            throw new System.ArgumentException($"Endpoint '{endpoint}' not found.");
        }
    }

    [System.Serializable]
    public struct EndpointDef
    {
        [SerializeField] private HttpVerb _verb;
        [SerializeField] private EndpointType _endpoint;
        [SerializeField] private string _url;
        [SerializeField] private string _contentType;
        [SerializeField] private HeaderPair[] _headers;

        public HttpVerb Verb => _verb;
        public EndpointType Endpoint => _endpoint;
        public string Url => _url;
        public string ContentType => _contentType;
        public HeaderPair[] Headers => _headers;
    }

    [System.Serializable]
    public struct HeaderPair
    {
        public string Key;
        public string Value;
    }
}
