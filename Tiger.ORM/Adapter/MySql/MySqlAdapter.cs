using System;
using System.Collections.Generic;
using System.Text;

namespace Tiger.ORM.Adapter
{
    public class MySqlAdapter : StandardAdapter, ISqlAdapter
    {
        public MySqlAdapter()
        {
            base.LeftPlaceholder = "`";
            base.RightPlaceholder = "`";
        }
    }
}
