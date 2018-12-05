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
using Tiger.ORM.Expressions;
using Tiger.ORM.Expressions.Query;
using Tiger.ORM.Utilities;
using Tiger.Test.Model;

namespace Tiger.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // List<string> list = new List<string>();


            //var a = list.Where(t => t == "").ToList();
            //IQueryable<Users>
            //a.OrderByDescending
            //SqlServerQueryable<TB_Customer_Info> queryable = new SqlServerQueryable<TB_Customer_Info>();
            ////queryable.Select(t => t.City,t.b,t.c).ToList()
            //queryable.Select(t => new { t.Id, t.City }).ToList();
            //TigerQueryable<TB_Customer_Info> queryable = new TigerQueryable<TB_Customer_Info>();
            //queryable.Select(t => new { t.Id, t.OpenId, t.NickName, t.Language }).FirstOrDefault();

            //queryable.Select(t => t.Id).FirstOrDefault();
            //queryable.Select(t => t.Id).FirstOrDefault();
            //queryable.SelectColumn(t => new { t.Id, t.CreateTime, t.NickName });
            //queryable.OrderBy(t => new { t.UpdateTime,t.City });
            //queryable.FirstOrDefault();

            using (TestContext context = new TestContext())
            {
                var a = context.Query<TB_Customer_Info>().Where(t => t.CreateTime <= DateTime.Now.AddDays(-1)).FirstOrDefault();
                //context.Query<TB_Customer_Info>().Count();
                //context.Query<TB_Customer_Info>().SelectColumn(t => t.BrandId).Count();
            }

        }


        static ISqlAdapter adapter = new SqlServerAdapter();
        //static void Main(string[] args)
        //{
        //    //AppConfig appConfig = new AppConfig()
        //    //{
        //    //    ConnectionName = "TigerDefault",
        //    //    ConnectionString = "",
        //    //    ProviderName = "MySql.Data.MySqlClient"
        //    //};
        //    //IDbConnection connection = ConnectionFactory.CreateConnection(appConfig);

        //    //Console.WriteLine(connection.GetType().FullName);
        //    //Users u = new Users()
        //    //{
        //    //    Id = Guid.NewGuid().ToString("d"),
        //    //    Name = "Ray雷2333",
        //    //    Height = 173
        //    //};

        //    //DeleteKey();

        //    //DeleteEntity(u);

        //    //InsertEntity(u);

        //    //UpdateEntity(u);
        //    //List<string> list = new List<string>();
        //    //list.Where(t => t == "asb").ToList();

        //    Class1 class1 = new Class1();
        //    class1.a();

        //    //using (TestContext context = new TestContext())
        //    //{
        //    //    //insert 
        //    //    //generate sql: INSERT ITNO user(id,UserName,Gender,Height) VALUES ('70eaf55c-099c-42d7-bc31-c49a92a29775','Ray',2,22)
        //    //    long inserResult = context.Insert(new Users()
        //    //    {
        //    //        Id = "70eaf55c-099c-42d7-bc31-c49a92a29775",
        //    //        Gender = 2,
        //    //        Name = "Ray",
        //    //        Height = 22
        //    //    });

        //    //    //update
        //    //    //generate sql: UPDATE user SET Gender=1,Height=25,UserName='Ray2' WHERE id='70eaf55c-099c-42d7-bc31-c49a92a29775'
        //    //    int updateResult1 = context.Update(new Users()
        //    //    {
        //    //        Gender = 1,
        //    //        Height = 25,
        //    //        Name = "Ray2",
        //    //        Id = "70eaf55c-099c-42d7-bc31-c49a92a29775"  //primary key it is necessary.
        //    //    });

        //    //    //update lambda
        //    //    //generate sql: UPDATE user SET UserName='Ray',Gender=2 WHERE id='70eaf55c-099c-42d7-bc31-c49a92a29775'
        //    //    int updateResult2 = context.Update<Users>(t => new Users
        //    //    {
        //    //        Name = "Ray",
        //    //        Gender = 2
        //    //    }).Where(t => t.Id == "70eaf55c-099c-42d7-bc31-c49a92a29775").Execute();

        //    //    //delete
        //    //    //generate sql: DELETE FROM user WHERE id='70eaf55c-099c-42d7-bc31-c49a92a29775'
        //    //    int deleteResult1 = context.Delete<Users>("70eaf55c-099c-42d7-bc31-c49a92a29775");

        //    //    //delete lambda
        //    //    //generate sql: DELETE FROM user WHERE UserName='Ray'
        //    //    int deleteResult2 = context.Delete<Users>()
        //    //                               .Where(t => t.Name == "Ray").Execute();
        //    //}

        //    Console.ReadKey();
        //}

        //static void QueryAllCustomer()
        //{
        //    using (TestContext context = new TestContext())
        //    {
        //        IEnumerable<TB_Customer_Info> result = context.Query<TB_Customer_Info>("select * from TB_Customer_Info");
        //        foreach (var item in result)
        //        {
        //            Console.WriteLine($"nickname:{item.NickName} \t ");
        //        }
        //    }
        //}



        //static void DeleteKey()
        //{
        //    string sql = adapter.Delete<Users>(Guid.NewGuid().ToString(), out DynamicParameters parameters);
        //    Console.WriteLine(sql);
        //    Console.WriteLine("parameters:");
        //    foreach (var item in parameters.ParameterNames)
        //    {
        //        Console.WriteLine("name:" + item + "\tvalue:" + parameters.Get<object>(item));
        //    }
        //    Console.WriteLine("-----------------------");
        //}

        //static void DeleteEntity(object entity)
        //{
        //    string deleteSql = adapter.Delete(entity, out DynamicParameters deleteparams);
        //    Console.WriteLine(deleteSql);
        //    Console.WriteLine("parameters:");
        //    foreach (var item in deleteparams.ParameterNames)
        //    {
        //        Console.WriteLine("name:" + item + "\tvalue:" + deleteparams.Get<object>(item));
        //    }
        //    Console.WriteLine("-----------------------");
        //}

        //static void InsertEntity(object entity)
        //{
        //    string insertSql = adapter.Insert(entity, out DynamicParameters insertPara);
        //    Console.WriteLine(insertSql);
        //    Console.WriteLine("parameters:");
        //    foreach (var item in insertPara.ParameterNames)
        //    {
        //        Console.WriteLine("name:" + item + "\tvalue:" + insertPara.Get<object>(item));
        //    }
        //    Console.WriteLine("-----------------------");
        //}

        //static void UpdateEntity(object entity)
        //{
        //    string sql = adapter.Update(entity, out DynamicParameters parameters);
        //    Console.WriteLine(sql);
        //    Console.WriteLine("parameters:");
        //    foreach (var item in parameters.ParameterNames)
        //    {
        //        Console.WriteLine("name:" + item + "\tvalue:" + parameters.Get<object>(item));
        //    }
        //    Console.WriteLine("-----------------------");
        //}


        //static void NewDbContext()
        //{
        //    using (TestContext context = new TestContext())
        //    {
        //        //context.Delete<Users>(t => t.Id == "123" && t.Name == "5465");
        //        //context.Update<Users>(t => new { Id = "123" ,Name = "456"});
        //        //long result = context.Insert<Users>(t => new Users()
        //        //{
        //        //    Id = "123",
        //        //    Height = 111,
        //        //    Name = "233223"
        //        //});
        //    }
        //}
    }
}
