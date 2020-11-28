using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.ORM.Expressions
{
    public class LambdaProperty
    {
        public PropertyInfo Property { get; set; }

        public object Value { get; set; }

        public string Operation { get; set; }
    }
}
