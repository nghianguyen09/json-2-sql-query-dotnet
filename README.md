JSON to SQL Query for MS SQL Server
==================

This library is ported from **JSON to SQL Query** at <https://github.com/xbsoftware/querysql> to C# & .NET Core to convert JSON config to SQL Query with named parameters for Microsoft SQL Server.

The Microsoft .NET Framework Data Provider for SQL Server **does not** support the question mark (?) placeholder for passing parameters to a SQL Statement. Named parameters must be used. See details [here](https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommand.parameters?view=dotnet-plat-ext-3.1#remarks)


## The following is copied from [xbsofware/querysql/README.md](https://github.com/xbsoftware/querysql#readme)

### Converts JSON config to SQL Query

```json
{
  "glue": "and",
  "rules": [{
    "field":"age",
    "condition":{
      "type": "less",
      "filter": 42
    } 
  },{
    "field":"region",
    "includes": [1,2,6]
  }] 
}
```

### Supported operations ( type )

- equal
- notEqual
- contains
- notContains
- lessOrEqual
- greaterOrEqual
- less
- notBetween
- between
- greater
- beginsWith
- notBeginsWith
- endsWith
- notEndsWith

### nesting

Blocks can be nested like next

```json
{
  "glue": "and",
  "rules": [
    ruleA,
    {
      "glue": "or",
      "rules": [
        ruleC,
        ruleD
      ] 
    }
  ] 
}
```

### between / notBeetween

For those operations, both start and end values can be provided

```json
{
    "field":"age",
    "condition":{
      "type": "between",
      "filter": { "start": 10, "end": 99 }
    } 
  }
```

if only *start* or *end* provided, the operation will change to *less* or *greater* automatically
