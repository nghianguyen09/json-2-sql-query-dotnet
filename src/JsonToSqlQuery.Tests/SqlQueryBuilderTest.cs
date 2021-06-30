using NUnit.Framework;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JsonToSqlQuery.Tests
{
    public class SqlQueryBuilderTest
    {
        private const string aAndB = "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'less', 'filter':1} }, { 'field': 'b', 'condition':{ 'type':'greater', 'filter':'abc' } }]}";
        private const string aOrB  =  "{ 'glue':'or', 'rules':[{ 'field': 'a', 'condition':{ 'type':'less', 'filter':1} }, { 'field': 'b', 'condition':{ 'type':'greater', 'filter':'abc' } }]}";

        static readonly object[] sqlQueryBuilderTestCases = 
        {
            new object[] {"{}", string.Empty, null},
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a'}]}",
                string.Empty,
                new Dictionary<string, JValue>()
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'' }}]}",
                string.Empty,
                new Dictionary<string, JValue>()
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'equal', 'filter':1 }}]}",
                "a = @p0",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'notEqual', 'filter':1 }}]}",
                "a <> @p0",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'contains', 'filter':'1' }}]}",
                "CHARINDEX(@p0, a) > 0",
                new Dictionary<string, JValue> { { "@p0", new JValue("1") } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'notContains', 'filter':'1' }}]}",
                "CHARINDEX(@p0, a) = 0",
                new Dictionary<string, JValue> { { "@p0", new JValue("1") } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'less', 'filter':1 }}]}",
		        "a < @p0",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'lessOrEqual', 'filter':1 }}]}",
                "a <= @p0",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'greater', 'filter':1 }}]}",
                "a > @p0",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'greaterOrEqual', 'filter':1 }}]}",
                "a >= @p0",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'beginsWith', 'filter':'1' }}]}",
                "a LIKE CONCAT(@p0, '%')",
                new Dictionary<string, JValue> { { "@p0", new JValue("1") } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'notBeginsWith', 'filter':'1' }}]}",
                "a NOT LIKE CONCAT(@p0, '%')",
                new Dictionary<string, JValue> { { "@p0", new JValue("1") } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'endsWith', 'filter':'1' }}]}",
                "a LIKE CONCAT('%', @p0)",
                new Dictionary<string, JValue> { { "@p0", new JValue("1") } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'notEndsWith', 'filter':'1' }}]}",
                "a NOT LIKE CONCAT('%', @p0)",
                new Dictionary<string, JValue> { { "@p0", new JValue("1") } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'between', 'filter':{ 'start':1, 'end':2 } }}]}",
                "a > @p0 AND a < @p1",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) }, { "@p1", new JValue(2) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'between', 'filter':{ 'start':1 } }}]}",
                "a > @p0",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'between', 'filter':{ 'end':2 } }}]}",
                "a < @p1",
                new Dictionary<string, JValue> { { "@p1", new JValue(2) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'notBetween', 'filter':{ 'start':1, 'end':2 } }}]}",
                "a < @p0 OR a > @p1",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) }, { "@p1", new JValue(2) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'notBetween', 'filter':{ 'start':1 } }}]}",
                "a < @p0",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'notBetween', 'filter':{ 'end':2 } }}]}",
                "a > @p1",
                new Dictionary<string, JValue> { { "@p1", new JValue(2) } }
            },
            new object[]
            {
                aOrB,
                "( a < @p0 OR b > @p1 )",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) }, { "@p1", new JValue("abc") } }
            },
            new object[]
            {
                "{ 'glue':'AND', 'rules':[" + aAndB + "," + aOrB + ", { 'field':'c', 'condition': { 'type':'equal', 'filter':3 } }]}",
                "( ( a < @p0 AND b > @p1 ) AND ( a < @p2 OR b > @p3 ) AND c = @p4 )",
                new Dictionary<string, JValue>
                { 
                    { "@p0", new JValue(1) }, { "@p1", new JValue("abc") },
                    { "@p2", new JValue(1) }, { "@p3", new JValue("abc") },
                    { "@p4", new JValue(3) }
                }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'includes':[1,2,3]}]}",
                "a IN (@p0,@p1,@p2)",
                new Dictionary<string, JValue> { { "@p0", new JValue(1) }, { "@p1", new JValue(2) }, { "@p2", new JValue(3) } }
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'includes':['a','b','c']}]}",
                "a IN (@p0,@p1,@p2)",
                new Dictionary<string, JValue> { { "@p0", new JValue("a") }, { "@p1", new JValue("b") }, { "@p2", new JValue("c") } }
            }
        };

        static readonly object[] sqlQueryBuilderTestCases_FromJSON_ReturnErrorResult =
        {
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', condition':{ 'type':'', 'filter':null } }] }",
                "Invalid JavaScript property identifier character: '."
            },

        };

        static readonly object[] sqlQueryBuilderTestCases_GetSQL_ReturnErrorResult =
        {
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'between', 'filter':5 }}]}",
                "wrong number of parameters for between operation: 1",
                null
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'notBetween', 'filter':5 }}]}",
                "wrong number of parameters for notBetween operation: 1",
                null
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'a', 'condition':{ 'type':'testUnsupportedOp', 'filter':5 }}]}",
                "unknown operation: testUnsupportedOp",
                null
            },
            new object[]
            {
                "{ 'glue':'and', 'rules':[{ 'field': 'c', 'condition':{ 'type':'equal', 'filter':1 }}]}",
                "field name is not in whitelist: c",
                new List<string> {"a", "b"}
            }
        };

        [Test, TestCaseSource(nameof(sqlQueryBuilderTestCases))]
        public void TestSqlQueryBuilder(string json, string expectedSql, IDictionary<string, JValue> expectedValues)
        {
            var sqlQueryBuilder = new SqlQueryBuilder();
            var filterResult = sqlQueryBuilder.FromJSON(json);
            if (filterResult.HasError)
            {
                Assert.Fail($"can't parse json\njson: {json}\n{filterResult.Error}");
            }

            var sqlResult = sqlQueryBuilder.GetSQL(filterResult.Filter);
            if (sqlResult.HasError)
            {
                Assert.Fail($"can't generate sql\njson: {json}\n{sqlResult.Error}");
            }

            //Assert.That(sqlResult.Sql, Is.EqualTo(expectedSql),
            //    $"wrong sql generated\njson: {json}");

            Assert.That(
                sqlResult.Sql == expectedSql,
                $"wrong sql generated\njson: {json}\ngenerated sql: {sqlResult.Sql}\nexpected sql: {expectedSql}"
            );

            //Assert.That(
            //    (sqlResult.Values == null && expectedValues == null)
            //        || ((sqlResult.Values != null && expectedValues != null)
            //            && (sqlResult.Values.Count == expectedValues.Count && !sqlResult.Values.Except(expectedValues).Any())),
            //    $"wrong values generated\njson: {json}\n" +
            //    $"generated values: {sqlResult.Values.ToDisplayString()}\n" +
            //    $"expected values: {expectedValues.ToDisplayString()}"
            //);
            Assert.That(sqlResult.Values.IsEqualTo(expectedValues),
                $"wrong values generated\njson: {json}\n" +
                $"generated values: {sqlResult.Values.ToDisplayString()}\n" +
                $"expected values: {expectedValues.ToDisplayString()}"
            );
        }
    
        [Test, TestCaseSource(nameof(sqlQueryBuilderTestCases_FromJSON_ReturnErrorResult))]
        public void TestSqlQueryBuilder_FromJSON_ReturnErrorResult(string json, string expectedErrorStartsWith)
        {
            var sqlQueryBuilder = new SqlQueryBuilder();
            var filterResult = sqlQueryBuilder.FromJSON(json);
            
            Assert.That(filterResult.HasError, Is.True);
            Assert.That(filterResult.Error, Is.Not.Null.And.StartsWith(expectedErrorStartsWith), $"Expected error message starts with: '{expectedErrorStartsWith}' returned, but it was not");
        }

        [Test, TestCaseSource(nameof(sqlQueryBuilderTestCases_GetSQL_ReturnErrorResult))]
        public void TestSqlQueryBuilder_GetSQL_ReturnErrorResult(string json, string expectedError, IList<string> whileList)
        {
            var sqlQueryBuilder = new SqlQueryBuilder(new Configuration.SqlConfig { WhileList = whileList });
            var filterResult = sqlQueryBuilder.FromJSON(json);
            Assert.That(filterResult.HasError, Is.False);

            var sqlResult = sqlQueryBuilder.GetSQL(filterResult.Filter);
            Assert.That(sqlResult.HasError, Is.True);
            Assert.That(sqlResult.Error, Is.Not.Null.And.StartsWith(expectedError), $"Expected error message: '{expectedError}' returned, but it was not");
        }
    }
}