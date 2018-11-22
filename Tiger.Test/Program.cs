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
            //Users u = new Users()
            //{
            //    Id = Guid.NewGuid().ToString("d"),
            //    Name = "Ray雷2333",
            //    Height = 173
            //};

            //DeleteKey();

            //DeleteEntity(u);

            //InsertEntity(u);

            //UpdateEntity(u);
            //List<string> list = new List<string>();
            //list.Where(t => t == "asb").ToList();

            using (TestContext context = new TestContext())
            {
                //context.Update<TB_Customer_Info>(t => new TB_Customer_Info
                //{
                //    Id = "123",
                //    NickName = "abcdk",
                //}).Where(t =>new { t.Id == "123"&&t.NickName = "" }).Execute();
                var a = context.Update<TB_Customer_Info>(t => new TB_Customer_Info
                {
                    Id = "123",
                    NickName = "abcdk",
                });
                a = a.Where(t => t.CreateTime == DateTime.Now);
                a = a.Where(t => t.UpdateTime == DateTime.MaxValue);
                int reuslt = a.Execute();

                context.Update<TB_Customer_Info>(t => new TB_Customer_Info
                {
                    Id = "123",
                    NickName = "abcdk",
                }).Where(t => t.Id == "123" && t.NickName == "ray").Execute();

                //context.Update<TB_Customer_Info>().Where().Execute();
            }

            //List<string> list = new List<string>();
            //list.Where(t => t == "123" && t == "456").ToList()

            //QueryAllCustomer();

            Console.ReadKey();
        }

        static void QueryAllCustomer()
        {
            using (TestContext context = new TestContext())
            {
                IEnumerable<TB_Customer_Info> result = context.Query<TB_Customer_Info>("select * from TB_Customer_Info");
                foreach (var item in result)
                {
                    Console.WriteLine($"nickname:{item.NickName} \t ");
                }
            }
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


        static void NewDbContext()
        {
            using (TestContext context = new TestContext())
            {
                //context.Delete<Users>(t => t.Id == "123" && t.Name == "5465");
                //context.Update<Users>(t => new { Id = "123" ,Name = "456"});
                //long result = context.Insert<Users>(t => new Users()
                //{
                //    Id = "123",
                //    Height = 111,
                //    Name = "233223"
                //});
            }
        }
    }
}
