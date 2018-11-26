using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tiger.ORM.Utilities;

namespace Tiger.ORM.Expressions.Query
{
    public class QueryEntity
    {
        public QueryEntity()
        {
            this.Top = -1;
        }

        public int Top { get; set; }

        public List<LambdaWhereEntity> QueryCondition { get; set; }

        public List<PropertyInfo> QueryColumn { get; set; }
        
        public List<PropertyInfo> Orderby { get; set; }

        public List<PropertyInfo> OrderbyDesc { get; set; }

        public List<PropertyInfo> Groupby { get; set; }
    }
}
