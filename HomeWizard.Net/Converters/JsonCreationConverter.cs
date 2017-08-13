using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HomeWizard.Net.Converters
{
    /// <summary>Base Generic JSON Converter that can help quickly define converters for specific types by automatically
    /// generating the CanConvert, ReadJson, and WriteJson methods, requiring the implementer only to define a strongly typed Create method.
    /// Adapted to support generic lists
    /// https://stackoverflow.com/questions/8030538/how-to-implement-custom-jsonconverter-in-json-net-to-deserialize-a-list-of-base </summary>
    internal abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>Create an instance of objectType, based properties in the JSON object</summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">contents of JSON object that will be deserialized</param>
        protected abstract T Create(Type objectType, JObject jObject);

        /// <summary>Determines if this converted is designed to deserialization to objects of the specified type.</summary>
        /// <param name="objectType">The target type for deserialization.</param>
        /// <returns>True if the type is supported.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        /// <summary>Parses the json to the specified type.</summary>
        /// <param name="reader">Newtonsoft.Json.JsonReader</param>
        /// <param name="objectType">Target type.</param>
        /// <param name="existingValue">Ignored</param>
        /// <param name="serializer">Newtonsoft.Json.JsonSerializer to use.</param>
        /// <returns>Deserialized Object</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                // Load JObject from stream
                JObject jObject = JObject.Load(reader);
                T target = CreateObject(reader, objectType, jObject, serializer);
                return target;
            }

            if (reader.TokenType == JsonToken.StartArray)
            {
                // Load JArray from stream
                JArray jArray = JArray.Load(reader);
                IList<T> targetList = new List<T>();
                foreach (JToken jToken in jArray)
                {
                    // Create JObject from JToken
                    JObject jObject = JObject.FromObject(jToken);
                    T target = CreateObject(reader, objectType, jObject, serializer);
                    targetList.Add(target);
                }
                return targetList;
            }

            return null;
        }

        /// <summary>Serializes to the specified type</summary>
        /// <param name="writer">Newtonsoft.Json.JsonWriter</param>
        /// <param name="value">Object to serialize.</param>
        /// <param name="serializer">Newtonsoft.Json.JsonSerializer to use.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        private T CreateObject(JsonReader reader, Type objectType, JObject jObject, JsonSerializer serializer)
        {
            // Create target object based on JObject
            T target = Create(objectType, jObject);

            // Create a new reader for this jObject, and set all properties to match the original reader.
            JsonReader jObjectReader = jObject.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;

            // Populate the object properties
            serializer.Populate(jObjectReader, target);

            return target;
        }

        protected bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }
}
