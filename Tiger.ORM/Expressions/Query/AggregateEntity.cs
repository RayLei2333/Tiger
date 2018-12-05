using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tiger.ORM.Expressions.Query
{
    internal class AggregateEntity
    {
        public PropertyInfo Property { get; set; }

        public AggregateEnum AggType { get; set; }
    }
}
