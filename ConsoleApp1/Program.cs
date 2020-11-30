using ConsoleApp1.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.SqlServer;
using Tiger.ORM.SqlServer.Adapter;

namespace ConsoleApp1
{
    class Program
    {
        public static string cc1 = "";

        static void Main(string[] args)
        {
            //DeleteLambdaTest();
            UpdateLambdaTest();
        }



        private static void DeleteLambdaTest()
        {

            using (DbContext context = new DbContext(new SqlConnection("")))
            {
                //context.Delete<T_WeChat_User>().Where(t => t.Id == cc1);
                //context.Delete<T_WeChat_User>().Where(t => t.CreateTime == DateTime.Now);
                //context.Delete<T_WeChat_User>().Where(t => t.CreateTime == new DateTime(2020, 10, 22, 12, 34, 12));
                //context.Delete<T_wehc>
                //context.Delete<T_WeChat_User>().Where(t => t.Gender == new T_WeChat_User().Gender);
                //context.Delete<T_WeChat_User>().Where(t => t.Gender == Convert.ToInt32("3"));
                context.Delete<T_WeChat_User>().Where(t => t.Gender == 1 && t.CreateTime == DateTime.Now).Execute();
                context.Delete<T_WeChat_User>().Execute();
            }
        }

        private static void UpdateLambdaTest()
        {
            using (DbContext db = new DbContext(new SqlConnection("")))
            {
                db.Update<T_WeChat_User>().Set(t => new T_WeChat_User
                {
                    Id = Guid.NewGuid().ToString("d"),
                    City = "ShangHai"
                });
            }
        }

        //INSERT INTO T_WeChat_User([Id],[NickName],[City],[Province],[Country],[Gender],[CreateTime]) VALUES 
        //(@Id0,@NickName0,@City0,@Province0,@Country0,@Gender0,@CreateTime0)
    }
}
