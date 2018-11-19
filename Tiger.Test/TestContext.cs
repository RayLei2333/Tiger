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
        public TestContext() : base("3321")
        {
        }
    }
}
