# DataBaseTool

Original Project URL
1. [Dapper](https://github.com/DapperLib/Dapper)
1. [Polly](https://github.com/App-vNext/Polly)

-------------
## Support
1. Support Retry - default for three times (first attempt after 1 second, second attempt after 1 second, third attempt after 2 seconds).
1. Support both synchronous and asynchronous operations.
1. Support selection of CommandType.
1. Support Transaction.

## How to use
Create a strong connection configuration first

### class
```csharp
public class ConnList
{
    public string db { get; set; } = string.Empty;
    public string SQLiteDatabase { get; set; } = string.Empty;
}
```

Create the setting
### appsetting.json

```json
"ConnectionStrings": {
    "db": "DataBase connection string"
}
```

Dependency Injection
### Program.cs
```csharp

var builder = WebApplication.CreateBuilder(args);
....

builder.Services.AddDataBase<ConnList>(builder.Configuration.GetSection("ConnectionStrings"));
```

Start for use
### Controller.cs
```csharp
using DataBaseTool.Services;

private readonly IDataBaseService _dataBaseService;

public TestController(IDataBaseService dataBaseService)
{
    _dataBaseService = dataBaseService;
}

public IActionResult Test()
{
    var cnt = _dataBaseService.QueryRetry<int>(sql, dataBase: "The selection of the corresponding database nae", commandType: System.Data.CommandType.Text, param: new { Name = "Test" }).FirstOrDefault();
    return Ok(cnt);
}

```