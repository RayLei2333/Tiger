using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.SqlServer;

namespace ConsoleApp1
{
    public class BaseResp : DbContext
    {
        public BaseResp() : base(new SqlConnection(""))
        {
        }
    }
}
