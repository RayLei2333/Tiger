using System;
using System.Collections.Generic;
using System.Text;

namespace Tiger.ORM.Adapter
{
    internal interface ISqlTimeHandle
    {
        string TimeFormat { get;}

        string TimeHandel(string timeValue, string style);
    }
}
