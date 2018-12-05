using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM;

namespace Tiger.Test
{
    public class TestContext : DbContext
    {
        public TestContext() : base(new SqlConnection("Data Source=121.40.186.190;Initial Catalog=Monkey;User ID=sa;Password=tt$#@!123;"))
        {
        }

        //public TestContext() : base(new MySqlConnection("connection string"))
        //{

        //}
    }
}
