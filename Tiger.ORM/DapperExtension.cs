using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Tiger.ORM.Adapter;
using Dapper;

namespace Tiger.ORM
{
    public static class DapperExtension
    {
        private static readonly ISqlAdapter DefaultAdapter = null;
        private static readonly Dictionary<string, ISqlAdapter> AdapterMap = new Dictionary<string, ISqlAdapter>()
        {
            ["sqlconnection"] = null,
            ["mysqlconnection"] = null
        };

        public static int Delete<T>(this IDbConnection connection, object key, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            ISqlAdapter adapter = GetAdapter(connection);
            OpenConnection(connection);
            string sql = adapter.Delete<T>(key, out DynamicParameters parameters);
            int result = connection.Execute(sql, parameters, transaction, commandTimeout, commandType);
            connection.Close();
            return result;
        }


        private static ISqlAdapter GetAdapter(IDbConnection connection)
        {
            string dbType = connection.GetType().Name.ToLower();
            if (AdapterMap.ContainsKey(dbType))
                return DefaultAdapter;
            return AdapterMap[dbType];
        }

        private static void OpenConnection(IDbConnection connection)
        {
            if (connection.State == ConnectionState.Broken || connection.State == ConnectionState.Closed)
                connection.Open();
        }
    }
}
