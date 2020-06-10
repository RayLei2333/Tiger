using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.ORM.ModelConfiguration.Exceptions
{
    public class ModelConfigException : Exception
    {
        public ModelConfigException(string msg) : base(msg)
        {
        }
    }
}
