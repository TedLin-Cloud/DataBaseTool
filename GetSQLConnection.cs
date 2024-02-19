using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace DataBaseTool.Services
{
    /// <summary>
    /// 取得連線字串
    /// </summary>
    public interface IGetSQLConnection
    {
        /// <summary>
        /// 取得MSSQL連線字串
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        SqlConnection GetSqlConn(string dataBase);
    }

    /// <summary>
    /// 取得連線字串
    /// </summary>
    /// <remarks>
    /// Initialization
    /// </remarks>
    /// <param name="conn"></param>
    public class GetSQLConnection<T>(IOptionsSnapshot<T> conn) : IGetSQLConnection where T : class
    {
        private readonly T _conn = conn.Value;

        /// <inheritdoc/>
        public SqlConnection GetSqlConn(string dataBase)
        {
            var strDBconn = (_conn.GetType()?.GetProperties()?.Where(x => x.Name == dataBase)?.FirstOrDefault()?.GetValue(_conn)?.ToString()) ?? throw new ArgumentException("查無連線資訊", nameof(dataBase));
            SqlConnectionStringBuilder builder = [];

            var itemsList = strDBconn.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).Select(item =>
            {
                var parts = item.Split('=');
                return new { Key = parts[0], Value = parts[1] };
            }).ToList();

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in itemsList)
            {
                dict.Add(line.Key, line.Value);
            }

            builder.DataSource = dict["Data Source"] ?? "";
            builder.InitialCatalog = dict["Initial Catalog"] ?? "";
            builder.UserID = dict["User Id"] ?? "";
            builder.Password = dict["Password"] ?? "";

            if (dict.TryGetValue("Application Name", out string ApplicationName))
            {
                builder.ApplicationName = dict["Application Name"];
            }

            builder.PersistSecurityInfo = true;
            strDBconn = builder.ConnectionString;
            return new SqlConnection(strDBconn);
        }
    }
}
