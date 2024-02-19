# DataBaseTool

DataBase應用擴充

-------------
## 功能
1. 支援Retry-共三次(第一次1秒後執行，第2次1秒，第三次2秒)
1. 支援同步及非同步
1. 支援CommandType
1. 支援RollBack

## 如何使用
請先建立強型別對應的連線設定

### class
```csharp
public class ConnList
{
    public string db { get; set; } = string.Empty;
    public string SQLiteDatabase { get; set; } = string.Empty;
}
```

建立對應的設定
### appsetting.json

```json
"ConnectionStrings": {
    "db": "DB connection string"
}
```

注入相關服務
### Program.cs
```csharp
using DataBaseTool.Services;

var builder = WebApplication.CreateBuilder(args);
....

builder.Services.AddDataBase<ConnList>(builder.Configuration.GetSection("ConnectionStrings"));
```

開始使用服務
可使用列舉方式穩定選擇DataBase
### Controller.cs
```csharp
private readonly IDataBaseService _dataBaseService;

public TestController(IDataBaseService dataBaseService)
{
    _dataBaseService = dataBaseService;
}

public IActionResult Test()
{
    var cnt = _dataBaseService.QueryRetry<int>(sql, dataBase: "對應的DB NAME", commandType: System.Data.CommandType.Text, param: new { Name = "Test" }).FirstOrDefault();
    return Ok(cnt);
}

```

| Version  | Author | Dependencies |  Last updated   | 說明 |
| ------------| ------------|------------|------------ | ------------ |
| 8.0.0  | Tedlin | Dapper:2.1.28 <br> Microsoft.Extensions.Logging.Abstractions:8.0.0<br> Microsoft.Extensions.Options.ConfigurationExtensions:8.0.0<br>Polly:8.3.0<br>System.Data.SqlClient:4.8.6<br>net8.0| 2024/02/19 |  |
