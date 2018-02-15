using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace netcore.Core.Extensions
{
    public static class JSONExtensions
    {
        /// <summary>
        /// Json stringify for the object
        /// </summary>
        /// <param name="obj">Object will be serialized.</param>
        /// <returns>Json string from serialized object</returns>
        public static string JsonStringify(this Object obj) 
        {
            var settings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// Deserialize json string into an object.
        /// </summary>
        /// <param name="jsonString">json string that will be deserialized.</param>
        /// <returns>Deserialized Object</returns>
        public static T ToJson<T>(this string jsonString) 
        {
            var settings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.DeserializeObject<T>(jsonString, settings);
        }

       
    }
}