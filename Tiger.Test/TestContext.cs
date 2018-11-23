using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM;

namespace Tiger.Test
{
    public class TestContext : DbContext
    {
        public TestContext() : base("name or connection")
        {
        }

        //public TestContext() : base(new MySqlConnection("connection string"))
        //{

        //}
    }
}
