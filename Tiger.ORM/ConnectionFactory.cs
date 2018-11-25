using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using Tiger.ORM.Utilities;

namespace Tiger.ORM
{
    internal class ConnectionFactory
    {
        private static readonly Dictionary<DbType, string> _providerMap = new Dictionary<DbType, string>()
        {
            [DbType.MySQL] = "MySql.Data.MySqlClient.MySqlConnection,MySql.Data"
        };

        public static IDbConnection CrateConnection(DbType db, string connection)
        {
            if (db == DbType.SQLServer)
                return new SqlConnection(connection);
            object[] parameters = new object[1];
            parameters[0] = connection;
            string classPath = _providerMap[db];
            Type o = Type.GetType(classPath);//加载类型
            object obj = Activator.CreateInstance(o, BindingFlags.Default, null, parameters, null, null);//根据类型创建实例
            return (IDbConnection)obj;
        }

        //private static readonly Dictionary<string, string> _providerMap = new Dictionary<string, string>()
        //{
        //    //["System.Data.SqlClient"] = "",
        //    ["MySql.Data.MySqlClient"] = "MySql.Data.MySqlClient.MySqlConnection,MySql.Data"
        //};

        //public static IDbConnection CreateConnection(AppConfig appConfig)
        //{
        //    if (appConfig.ProviderName == "System.Data.SqlClient")
        //        return new SqlConnection(appConfig.ConnectionString);

        //    object[] parameters = new object[1];
        //    parameters[0] = appConfig.ConnectionString;
        //    string classPath = _providerMap[appConfig.ProviderName];
        //    Type o = Type.GetType(classPath);//加载类型
        //    object obj = Activator.CreateInstance(o, BindingFlags.Default, null, parameters, null, null);//根据类型创建实例

        //    return (IDbConnection)obj;
        //}
    }
}
