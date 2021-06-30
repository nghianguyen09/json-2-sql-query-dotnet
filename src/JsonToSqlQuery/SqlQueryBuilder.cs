using JsonToSqlQuery.Configuration;
using JsonToSqlQuery.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace JsonToSqlQuery
{
    public class SqlQueryBuilder
    {
        private readonly SqlConfig sqlConfig;
        
        public SqlQueryBuilder() { }

        public SqlQueryBuilder(SqlConfig sqlConfig)
        {
            this.sqlConfig = sqlConfig;
        }

        public JsonToFilterResult FromJSON(string query)
        {
            Filter filter = null;
            var errors = new List<string>();

            if (!string.IsNullOrWhiteSpace(query))
            {
                filter = JsonConvert.DeserializeObject<Filter>(query, new JsonSerializerSettings
                {
                    Error = delegate (object sender, ErrorEventArgs args)
                    {
                        errors.Add(args.ErrorContext.Error.Message);
                        args.ErrorContext.Handled = true;
                    }
                });
            }

            var error = errors.Count > 0 ? string.Join(", ", errors) : null;
            
            return new JsonToFilterResult
            {
                Filter = filter,
                Error = error
            };
        }

        public GetSqlResult GetSQL(Filter filter)
        {
            int pCount = 0;
            return GetSQL(filter, ref pCount);
        }

        private GetSqlResult GetSQL(Filter filter, ref int pCount)
        {
            var outputSql = new List<string>();
            IDictionary<string, JValue> values = new Dictionary<string, JValue>();

            if (filter.Kids == null)
            {
                if (sqlConfig != null && sqlConfig.WhileList != null && !sqlConfig.WhileList.Contains(filter.Field))
                {
                    return new GetSqlResult(error: $"field name is not in whitelist: {filter.Field}");
                }

                if (filter.Includes != null && filter.Includes.Count > 0)
                {
                    var inSql = GetInSQL(filter.Field, filter.Includes, values, ref pCount);
                    return new GetSqlResult(sql: inSql, values: values);
                }

                if (filter.Condition == null)
                {
                    return new GetSqlResult(sql: string.Empty, null);
                }

                var pName = $"@p{pCount++}";
                var pName2 = "";
                var conditionValues = filter.Condition.GetValues();

                if (conditionValues.Count >= 1)
                {
                    values.Add(pName, conditionValues[0]);
                };

                // case of Between & NotBetween
                if (conditionValues.Count == 2)
                {
                    pName2 = $"@p{pCount++}";
                    values.Add(pName2, conditionValues[1]);
                }

                switch (filter.Condition.Rule)
                {
                    case "":
                        return new GetSqlResult(sql: string.Empty, values: values);

                    case "equal":
                        return new GetSqlResult(sql: $"{filter.Field} = {pName}", values: values);             

                    case "notEqual":
                        return new GetSqlResult(sql: $"{filter.Field} <> {pName}", values: values);

                    case "contains":
                        return new GetSqlResult(sql: $"CHARINDEX({pName}, {filter.Field}) > 0", values: values);

                    case "notContains":
                        return new GetSqlResult(sql: $"CHARINDEX({pName}, {filter.Field}) = 0", values: values);

                    case "less":
                        return new GetSqlResult(sql: $"{filter.Field} < {pName}", values: values);

                    case "lessOrEqual":
                        return new GetSqlResult(sql: $"{filter.Field} <= {pName}", values: values);

                    case "greater":
                        return new GetSqlResult(sql: $"{filter.Field} > {pName}", values: values);

                    case "greaterOrEqual":
                        return new GetSqlResult(sql: $"{filter.Field} >= {pName}", values: values);

                    case "beginsWith":
                        var search = $"CONCAT({pName}, '%')";
                        return new GetSqlResult(sql: $"{filter.Field} LIKE {search}", values: values);

                    case "notBeginsWith":
                        search = $"CONCAT({pName}, '%')";
                        return new GetSqlResult(sql: $"{filter.Field} NOT LIKE {search}", values: values);

                    case "endsWith":
                        search = $"CONCAT('%', {pName})";
                        return new GetSqlResult(sql: $"{filter.Field} LIKE {search}", values: values);

                    case "notEndsWith":
                        search = $"CONCAT('%', {pName})";
                        return new GetSqlResult(sql: $"{filter.Field} NOT LIKE {search}", values: values);

                    case "between":
                        if (conditionValues.Count != 2)
                        {
                            return new GetSqlResult(error: $"wrong number of parameters for between operation: {conditionValues.Count}");
                        }

                        if (conditionValues[0] == null)
                        {
                            values.Remove(pName);
                            return new GetSqlResult(sql: $"{filter.Field} < {pName2}", values: values);
                        }
                        else if (conditionValues[1] == null)
                        {
                            values.Remove(pName2);
                            return new GetSqlResult(sql: $"{filter.Field} > {pName}", values: values);
                        }
                        else
                        {
                            return new GetSqlResult(sql: $"{filter.Field} > {pName} AND {filter.Field} < {pName2}", values: values);
                        }

                    case "notBetween":
                        if (conditionValues.Count != 2)
                        {
                            return new GetSqlResult(error: $"wrong number of parameters for notBetween operation: {conditionValues.Count}");
                        }

                        if (conditionValues[0] == null)
                        {
                            values.Remove(pName);
                            return new GetSqlResult(sql: $"{filter.Field} > {pName2}", values: values);
                        }
                        else if (conditionValues[1] == null)
                        {
                            values.Remove(pName2);
                            return new GetSqlResult(sql: $"{filter.Field} < {pName}", values: values);
                        }
                        else
                        {
                            return new GetSqlResult(sql: $"{filter.Field} < {pName} OR {filter.Field} > {pName2}", values: values);
                        }                    
                }

                return new GetSqlResult(error: $"unknown operation: {filter.Condition.Rule}");
            }

            foreach (var k in filter.Kids)
            {
                var subSqlResult = GetSQL(k, ref pCount);
                if (subSqlResult.HasError)
                {
                    return subSqlResult;
                }

                if (string.IsNullOrEmpty(subSqlResult.Sql))
                {
                    continue;
                }

                outputSql.Add(subSqlResult.Sql);

                foreach (var kv in subSqlResult.Values)
                {
                    values.Add(kv);
                }
            }

            string glue;
            if ("or".Equals(filter.Glue, System.StringComparison.InvariantCultureIgnoreCase))
            {
                glue = " OR ";
            }
            else
            {
                glue = " AND ";
            }

            var outStr = string.Join(glue, outputSql);

            if (filter.Kids.Count > 1)
            {
                outStr = "( " + outStr + " )";
            }

            return new GetSqlResult(sql: outStr, values: values);
        }

        private string GetInSQL(string field, IList<JValue> data, IDictionary<string, JValue> values, ref int pCount)
        {
            var pNames = new List<string>();

            foreach (var v in data)
            {
                string pName = $"@p{pCount++}";
                pNames.Add(pName);

                values.Add(pName, v);
            }

            var sql = $"{field} IN ({string.Join(",", pNames)})";
            return sql;
        }
    }
}
