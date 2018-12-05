using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Tiger.ORM.Expressions
{
    public interface ITigerQueryable<T>
    {
        ITigerQueryable<T> SelectColumn(Expression<Func<T, object>> selector);

        ITigerQueryable<T> Where(Expression<Func<T, bool>> predicate);

        ITigerQueryable<T> OrderBy(Expression<Func<T, object>> keySelector);


        int Count();

        IEnumerable<T> AsEnumerable();

        List<T> ToList();


        T FirstOrDefault();
    }
}
