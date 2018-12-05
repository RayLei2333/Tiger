using System;
using System.Collections.Generic;
using System.Text;

namespace Tiger.ORM.Adapter.SqlServer
{
    public class SqlServerTimeHandle : ISqlTimeHandle
    {
        public string TimeFormat
        {
            get { return "CONVERT(VARCHAT(10),{0},{1})"; }
        }

        public string TimeHandel(string timeValue, string style)
        {
            return string.Format(this.TimeFormat, timeValue, style);
        }
    }
}
