using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TestTask_CKR_System.Core.Utils.Serializer
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);
        T Deserialize<T>(string json);
        object Deserialize(string json, Type t);
    }

    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public string Serialize(object obj) => JsonConvert.SerializeObject(obj, _settings);
        public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _settings);
        public object Deserialize(string json, Type t) => JsonConvert.DeserializeObject(json, t, _settings);

    }
}