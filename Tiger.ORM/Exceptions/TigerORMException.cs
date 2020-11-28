using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.ORM.Exceptions
{
    public class TigerORMException : Exception
    {
        public TigerORMException(string msg) : base(msg) { }
    }
}
