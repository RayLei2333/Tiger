using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Tiger.ORM.Adapter;
using Dapper;

namespace Tiger.ORM
{
    internal static class DapperExtension
    {
        private static readonly ISqlAdapter DefaultAdapter = new SqlServerAdapter();
        private static readonly Dictionary<string, ISqlAdapter> AdapterMap = new Dictionary<string, ISqlAdapter>()
        {
            ["sqlconnection"] = new SqlServerAdapter(),
            ["mysqlconnection"] = new MySqlAdapter()
        };

        public static int Insert(this IDbConnection connection, object entity, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            ISqlAdapter adapter = GetAdapter(connection);
            //OpenConnection(connection);
            string sql = adapter.Insert(entity, out DynamicParameters parameters);
            int result = connection.Execute(sql, parameters, transaction, commandTimeout, commandType);
            //connection.Close();
            return result;
        }
        
        public static int Delete<T>(this IDbConnection connection, object key, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            ISqlAdapter adapter = GetAdapter(connection);
            string sql = adapter.Delete<T>(key, out DynamicParameters parameters);
            int result = connection.Execute(sql, parameters, transaction, commandTimeout, commandType);
            return result;
        }

        public static int Update(this IDbConnection connection,object entity,IDbTransaction transaction = null,int? commandTimeout = null,CommandType? commandType = null)
        {
            ISqlAdapter adapter = GetAdapter(connection);
            string sql = adapter.Update(entity, out DynamicParameters parameters);
            int result = connection.Execute(sql, parameters, transaction, commandTimeout, commandType);
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
