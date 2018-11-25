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
    }
}
