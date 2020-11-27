using ConsoleApp1.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.SqlServer.Adapter;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            T_WeChat_User u = new T_WeChat_User();
            SqlAdapter adapter = new SqlAdapter();
            DynamicParameters para = new DynamicParameters();
            string sql = adapter.Insert(u, para);
            Console.WriteLine(sql);
            IEnumerable<string> paraName = para.ParameterNames;
            Console.WriteLine("参数");
            foreach (var item in paraName)
            {
                Console.WriteLine($"{item} = {para.Get<object>(item)}");
            }
            Console.ReadKey();
        }

        //INSERT INTO T_WeChat_User([Id],[NickName],[City],[Province],[Country],[Gender],[CreateTime]) VALUES 
        //(@Id0,@NickName0,@City0,@Province0,@Country0,@Gender0,@CreateTime0)
    }
}
