using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace JsonToSqlQuery.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Filter
    {
        public string Glue { get; set; }

        public string Field { get; set; }

        public Condition Condition { get; set; }

        public IList<JValue> Includes { get; set; }

        [JsonProperty("rules")]
        public IList<Filter> Kids { get; set; }
    }
}