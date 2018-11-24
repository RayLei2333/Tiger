using System;
using System.Collections.Generic;
using System.Text;

namespace Tiger.ORM.Exceptions
{
    public class TigerORMException : Exception
    {
        public TigerORMException(string msg) : base(msg) { }
    }
}
