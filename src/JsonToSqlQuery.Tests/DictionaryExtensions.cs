using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace JsonToSqlQuery.Tests
{
    public static class DictionaryExtensions
    {
        public static string ToDisplayString(this IDictionary<string, JValue> dic)
        {
            return
                dic == null ? "null" :
                dic.Count == 0 ? "empty" : string.Join(',', dic.Select(kv => $"key: {kv.Key}, value: {kv.Value}"));
        }

        public static bool IsEqualTo(this IDictionary<string, JValue> dic1, IDictionary<string, JValue> dic2)
        {
            return
                (dic1 == null && dic2 == null)
                || (dic1 != null && dic2 != null && dic1.Count == dic2.Count && !dic1.Except(dic2).Any());
        }
    }
}
