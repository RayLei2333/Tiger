using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tiger.ORM.Expressions
{
    public interface ITigerQueryable<T>
    {



        IEnumerable<T> AsEnumerable();

        List<T> ToList();
    }
}
