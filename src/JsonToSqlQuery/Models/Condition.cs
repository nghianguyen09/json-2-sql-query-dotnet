using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JsonToSqlQuery.Models
{
    public class Condition
    {
        [JsonProperty("type")]
        public string Rule { get; set; }

        [JsonProperty("filter")]
        public JToken Value { get; set; }

        public IList<JValue> GetValues()
        {
            var values = new List<JValue>();

            if (Value is JObject)
            {
                var jObject = Value as JObject;
                values.Add((JValue)jObject.GetValue("start"));
                values.Add((JValue)jObject.GetValue("end"));
            }
            else if (Value is JValue value)
            {
                // single value
                values.Add(value);
            }

            return values;
        }
    }
}