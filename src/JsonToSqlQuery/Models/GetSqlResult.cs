using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JsonToSqlQuery.Models
{
    public class GetSqlResult
    {
        public GetSqlResult(string sql, IDictionary<string, JValue> values)
        {
            Sql = sql;
            Values = values;
            Error = null;
        }

        public GetSqlResult(string error)
        {
            Sql = string.Empty;
            Values = null;
            Error = error;
        }

        public string Sql { get; set; }

        public IDictionary<string, JValue> Values { get; set; }

        public string Error { get; set; }

        public bool HasError
        {
            get { return !string.IsNullOrEmpty(Error); }
        }
    }
}
