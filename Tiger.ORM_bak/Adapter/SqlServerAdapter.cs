using System;
using System.Collections.Generic;
using System.Text;

namespace Tiger.ORM.Adapter
{
    public class SqlServerAdapter : StandardAdapter, ISqlAdapter
    {
        public SqlServerAdapter()
        {
            base.LeftPlaceholder = "[";
            base.RightPlaceholder = "]";
        }
    }
}
