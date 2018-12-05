using System;
using System.Collections.Generic;
using System.Text;

namespace Tiger.ORM.Adapter.MySql
{
    public class MySqlTimeHandle : ISqlTimeHandle
    {
        public string TimeFormat
        {
            get
            {
                return "str_to_date({0},{1})";
            }
        }

        public string TimeHandel(string timeValue, string style)
        {
            return string.Format(this.TimeFormat, timeValue, style);
        }
    }
}
