using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonDataGridView
{
    public static class JsonExtensions
    {
        public static T As<T>(this JObject jobj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(jobj));
        }

        public static List<T> ToList<T>(this JArray jarray)
        {
            return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(jarray));
        }
    }
}