using DataBaseTool.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataBaseTool
    {
        /// <summary>
        /// sdsd
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="Services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddDataBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(
       this IServiceCollection Services, IConfiguration config) where TOptions : class
        {
            Services.Configure<TOptions>(config);
            Services.TryAddScoped<IGetSQLConnection, GetSQLConnection<TOptions>>();
            Services.TryAddScoped<IDataBaseService, DataBaseService>();
            return Services;
        }
    }
}