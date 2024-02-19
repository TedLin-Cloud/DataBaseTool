using Dapper;
using Microsoft.Extensions.Logging;
using Polly;
using System.Data;

namespace DataBaseTool.Services
{
    /// <summary>
    /// DB工具
    /// </summary>
    public interface IDataBaseService
    {
        #region Query
        /// <summary>
        /// 查詢，失敗Rty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SqlText"></param>
        /// <param name="dataBase"></param>
        /// <param name="commandType"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        IEnumerable<T> QueryRetry<T>(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, TimeSpan[] timeSpans = null);
        /// <summary>
        /// 查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SqlText"></param>
        /// <param name="dataBase"></param>
        /// <param name="commandType"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        IEnumerable<T> QueryNoRetry<T>(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false);
        /// <summary>
        /// 查詢，失敗Rty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SqlText"></param>
        /// <param name="dataBase"></param>
        /// <param name="commandType"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="OutputStr"></param>
        /// <param name="Output"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsyncRetry<T>(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, TimeSpan[] timeSpans = null);
        /// <summary>
        /// 查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SqlText"></param>
        /// <param name="dataBase"></param>
        /// <param name="commandType"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsyncNoRetry<T>(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false);
        #endregion

        #region 查詢及回傳Output
        /// <summary>
        /// 查詢及回傳Output，失敗Rty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SqlText"></param>
        /// <param name="dataBase"></param>
        /// <param name="commandType"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="OutputStr"></param>
        /// <param name="Output"></param>
        /// <returns></returns>
        IEnumerable<T> QueryRetry<T>(string SqlText, string dataBase, out string Output, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, string OutputStr = "rtnCode", TimeSpan[] timeSpans = null);
        IEnumerable<T> QueryNoRetry<T>(string SqlText, string dataBase, out string Output, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, string OutputStr = "rtnCode");
        #endregion

        #region Execute
        /// <summary>
        /// 回傳影響數量
        /// </summary>
        /// <param name="SqlText"></param>
        /// <param name="dataBase"></param>
        /// <param name="commandType"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int ExecuteRetry(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, TimeSpan[] timeSpans = null);
        /// <summary>
        /// 回傳影響數量
        /// </summary>
        /// <param name="SqlText"></param>
        /// <param name="dataBase"></param>
        /// <param name="commandType"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int ExecuteNoRetry(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false);
        /// <summary>
        /// 回傳影響數量
        /// </summary>
        /// <param name="SqlText"></param>
        /// <param name="dataBase"></param>
        /// <param name="commandType"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<int> ExecuteAsyncRetry(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, TimeSpan[] timeSpans = null);
        /// <summary>
        /// 回傳影響數量
        /// </summary>
        /// <param name="SqlText"></param>
        /// <param name="dataBase"></param>
        /// <param name="commandType"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<int> ExecuteAsyncNoRetry(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false);
        #endregion
    }
    /// <summary>
    /// DB工具
    /// </summary>
    /// <remarks>
    /// 初始化
    /// </remarks>
    /// <param name="logger"></param>
    /// <param name="getSQLConnection"></param>
    public class DataBaseService(ILogger<DataBaseService> logger, IGetSQLConnection getSQLConnection) : IDataBaseService
    {

        #region Query
        /// <inheritdoc/>
        public IEnumerable<T> QueryRetry<T>(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, TimeSpan[] timeSpans = null)
        {
            timeSpans ??= DefaultRetryTimespan();
            return Policy.Handle<Exception>().WaitAndRetry(timeSpans, (response, retryTime, context) =>
            {
                var errorMsg = response?.Message;
                logger.LogDebug(message: "延遲重試，發生錯誤：{errorMsg}，延遲 {retryTime} 後重試", errorMsg, retryTime);
            }).Execute(() => Query<T>(SqlText, dataBase, commandType, param, transaction));
        }
        /// <inheritdoc/>
        public IEnumerable<T> QueryNoRetry<T>(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false)
        {
            return Query<T>(SqlText, dataBase, commandType, param, transaction);
        }
        /// <inheritdoc/>
        public Task<IEnumerable<T>> QueryAsyncRetry<T>(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, TimeSpan[] timeSpans = null)
        {
            timeSpans ??= DefaultRetryTimespan();
            return Policy.Handle<Exception>().WaitAndRetryAsync(timeSpans, (response, retryTime, context) =>
            {
                var errorMsg = response?.Message;
                logger.LogDebug(message: "延遲重試，發生錯誤：{errorMsg}，延遲 {retryTime} 後重試", errorMsg, retryTime);
            }).ExecuteAsync(() => QueryAsync<T>(SqlText, dataBase, commandType, param, transaction));
        }
        /// <inheritdoc/>
        public Task<IEnumerable<T>> QueryAsyncNoRetry<T>(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false)
        {
            return QueryAsync<T>(SqlText, dataBase, commandType, param, transaction);
        }
        #endregion

        #region QueryOutPut
        /// <inheritdoc/>
        public IEnumerable<T> QueryRetry<T>(string SqlText, string dataBase, out string Output, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, string OutputStr = "rtnCode", TimeSpan[] timeSpans = null)
        {
            timeSpans ??= DefaultRetryTimespan();
            var result_OutPut = string.Empty;
            var result = Policy.Handle<Exception>().WaitAndRetry(timeSpans, (response, retryTime, context) =>
            {
                var errorMsg = response?.Message;
                logger.LogDebug(message: "延遲重試，發生錯誤：{errorMsg}，延遲 {retryTime} 後重試", errorMsg, retryTime);
            }).Execute(() => QueryOutPut<T>(SqlText, dataBase, out result_OutPut, commandType, param, transaction, OutputStr));
            Output = result_OutPut;
            return result;
        }
        /// <inheritdoc/>
        public IEnumerable<T> QueryNoRetry<T>(string SqlText, string dataBase, out string Output, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, string OutputStr = "rtnCode")
        {
            var result = QueryOutPut<T>(SqlText, dataBase, out string result_OutPut, commandType, param, transaction, OutputStr);
            Output = result_OutPut;
            return result;
        }
        #endregion

        #region Execute
        /// <inheritdoc/>
        public int ExecuteRetry(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, TimeSpan[] timeSpans = null)
        {
            timeSpans ??= DefaultRetryTimespan();
            return Policy.Handle<Exception>().WaitAndRetry(timeSpans, (response, retryTime, context) =>
            {
                var errorMsg = response?.Message;
                logger.LogDebug(message: "延遲重試，發生錯誤：{errorMsg}，延遲 {retryTime} 後重試", errorMsg, retryTime);
            }).Execute(() => Execute(SqlText, dataBase, commandType, param, transaction));
        }

        /// <inheritdoc/>
        public int ExecuteNoRetry(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false)
        {
            return Execute(SqlText, dataBase, commandType, param, transaction);
        }

        /// <inheritdoc/>
        public Task<int> ExecuteAsyncRetry(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, TimeSpan[] timeSpans = null)
        {
            timeSpans ??= DefaultRetryTimespan();
            return Policy.Handle<Exception>().WaitAndRetryAsync(timeSpans, (response, retryTime, context) =>
            {
                var errorMsg = response?.Message;
                logger.LogDebug(message: "延遲重試，發生錯誤：{errorMsg}，延遲 {retryTime} 後重試", errorMsg, retryTime);
            }).ExecuteAsync(() => ExecuteAsync(SqlText, dataBase, commandType, param, transaction));
        }

        /// <inheritdoc/>
        public Task<int> ExecuteAsyncNoRetry(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false)
        {
            return ExecuteAsync(SqlText, dataBase, commandType, param, transaction);
        }
        #endregion

        #region 預設參數
        private static TimeSpan[] DefaultRetryTimespan() => new[]
                                                          {
                                              TimeSpan.FromSeconds(1),
                                              TimeSpan.FromSeconds(1),
                                              TimeSpan.FromSeconds(2)
                                          };
        #endregion

        #region 實作
        private IEnumerable<T> Query<T>(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false)
        {
            if (transaction)
                return SQLExcuteTrans((conn, trans) => conn.Query<T>(sql: SqlText, param: param, transaction: trans, commandType: commandType), dataBase);
            else
                return SQLExcute((conn) => conn.Query<T>(sql: SqlText, param: param, commandType: commandType), dataBase);
        }

        private IEnumerable<T> QueryOutPut<T>(string SqlText, string dataBase, out string Output, CommandType commandType = CommandType.Text, object param = null, bool transaction = false, string OutputStr = "rtnCode")
        {
            var result = Enumerable.Empty<T>();
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.AddDynamicParams(param);
            dynamicParameters.Add(OutputStr, dbType: DbType.String, direction: ParameterDirection.Output, size: 10);
            if (transaction)
                result = SQLExcuteTrans((conn, trans) => conn.Query<T>(sql: SqlText, param: dynamicParameters, transaction: trans, commandType: commandType), dataBase);
            else
                result = SQLExcute((conn) => conn.Query<T>(sql: SqlText, param: dynamicParameters, commandType: commandType), dataBase);
            try
            {
                Output = dynamicParameters.Get<string>(@$"@{OutputStr}").ToString().Trim();
            }
            catch (Exception)
            {
                Output = "";
            }

            return result;
        }

        private Task<IEnumerable<T>> QueryAsync<T>(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false)
        {
            if (transaction)
                return SQLExcuteTransAsync((conn, trans) => conn.QueryAsync<T>(sql: SqlText, param: param, transaction: trans, commandType: commandType), dataBase);
            else
                return SQLExcuteAsync((conn) => conn.QueryAsync<T>(sql: SqlText, param: param, commandType: commandType), dataBase);
        }

        private int Execute(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false)
        {
            if (transaction)
                return SQLExcuteTrans((conn, trans) => conn.Execute(sql: SqlText, param: param, transaction: trans, commandType: commandType), dataBase);
            else
                return SQLExcute((conn) => conn.Execute(sql: SqlText, param: param, commandType: commandType), dataBase);
        }

        private Task<int> ExecuteAsync(string SqlText, string dataBase, CommandType commandType = CommandType.Text, object param = null, bool transaction = false)
        {
            if (transaction)
                return SQLExcuteTransAsync((conn, trans) => conn.ExecuteAsync(sql: SqlText, param: param, transaction: trans, commandType: commandType), dataBase);
            else
                return SQLExcuteAsync((conn) => conn.ExecuteAsync(sql: SqlText, param: param, commandType: commandType), dataBase);
        }

        private T SQLExcute<T>(Func<IDbConnection, T> sqlExcutor, string dataBase)
        {
            using IDbConnection conn = getSQLConnection.GetSqlConn(dataBase);
            return sqlExcutor(conn);
        }

        private async Task<T> SQLExcuteAsync<T>(Func<IDbConnection, Task<T>> sqlExcutor, string dataBase)
        {
            using IDbConnection conn = getSQLConnection.GetSqlConn(dataBase);
            return await sqlExcutor(conn);
        }

        private T SQLExcuteTrans<T>(Func<IDbConnection, IDbTransaction, T> sqlExcutor, string dataBase)
        {
            using IDbConnection conn = getSQLConnection.GetSqlConn(dataBase);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                var result = sqlExcutor(conn, tran);
                tran.Commit();
                return result;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        private async Task<T> SQLExcuteTransAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> sqlExcutor, string dataBase)
        {
            using var conn = getSQLConnection.GetSqlConn(dataBase);
            await conn.OpenAsync();
            using var tran = await conn.BeginTransactionAsync();
            try
            {
                var result = await sqlExcutor(conn, tran);
                await tran.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                await tran.RollbackAsync();
                throw;
            }
        }
        #endregion
    }
}
