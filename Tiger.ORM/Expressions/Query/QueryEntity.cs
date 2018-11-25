using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tiger.ORM.Utilities;

namespace Tiger.ORM.Expressions.Query
{
    internal class QueryEntity
    {
        public QueryEntity()
        {
        }

        public List<LambdaWhereEntity> QueryCondition { get; set; }

        public List<PropertyInfo> QueryColumn { get; set; }
        
        public List<PropertyInfo> Orderby { get; set; }

        public List<PropertyInfo> OrderbyDesc { get; set; }

        public List<PropertyInfo> Groupby { get; set; }
    }
}
