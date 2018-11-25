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
        public TestContext() : base(new SqlConnection(""))
        {
        }

        //public TestContext() : base(new MySqlConnection("connection string"))
        //{

        //}
    }
}
