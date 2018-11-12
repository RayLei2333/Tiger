using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM;
using Tiger.ORM.Adapter;
using Tiger.ORM.Utilities;
using Tiger.Test.Model;

namespace Tiger.Test
{
    class Program
    {
        static ISqlAdapter adapter = new SqlServerAdapter();
        static void Main(string[] args)
        {
            //AppConfig appConfig = new AppConfig()
            //{
            //    ConnectionName = "TigerDefault",
            //    ConnectionString = "",
            //    ProviderName = "MySql.Data.MySqlClient"
            //};
            //IDbConnection connection = ConnectionFactory.CreateConnection(appConfig);

            //Console.WriteLine(connection.GetType().FullName);
            Users u = new Users()
            {
                Id = Guid.NewGuid().ToString("d"),
                Name = "Ray雷2333",
                Height = 173
            };

            DeleteKey();

            DeleteEntity(u);

            InsertEntity(u);

            UpdateEntity(u);


            Console.ReadKey();
        }

        static void DeleteKey()
        {
            string sql = adapter.Delete<Users>(Guid.NewGuid().ToString(), out DynamicParameters parameters);
            Console.WriteLine(sql);
            Console.WriteLine("parameters:");
            foreach (var item in parameters.ParameterNames)
            {
                Console.WriteLine("name:" + item + "\tvalue:" + parameters.Get<object>(item));
            }
            Console.WriteLine("-----------------------");
        }

        static void DeleteEntity(object entity)
        {
            string deleteSql = adapter.Delete(entity, out DynamicParameters deleteparams);
            Console.WriteLine(deleteSql);
            Console.WriteLine("parameters:");
            foreach (var item in deleteparams.ParameterNames)
            {
                Console.WriteLine("name:" + item + "\tvalue:" + deleteparams.Get<object>(item));
            }
            Console.WriteLine("-----------------------");
        }

        static void InsertEntity(object entity)
        {
            string insertSql = adapter.Insert(entity, out DynamicParameters insertPara);
            Console.WriteLine(insertSql);
            Console.WriteLine("parameters:");
            foreach (var item in insertPara.ParameterNames)
            {
                Console.WriteLine("name:" + item + "\tvalue:" + insertPara.Get<object>(item));
            }
            Console.WriteLine("-----------------------");
        }

        static void UpdateEntity(object entity)
        {
            string sql = adapter.Update(entity, out DynamicParameters parameters);
            Console.WriteLine(sql);
            Console.WriteLine("parameters:");
            foreach (var item in parameters.ParameterNames)
            {
                Console.WriteLine("name:" + item + "\tvalue:" + parameters.Get<object>(item));
            }
            Console.WriteLine("-----------------------");
        }

    }
}
