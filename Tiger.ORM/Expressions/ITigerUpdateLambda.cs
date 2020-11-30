using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.ORM.Expressions
{
    public interface ITigerUpdateLambda<T> : ITigerLambda<T>
    {
        ITigerUpdateLambda<T> Set(Expression<Func<T, T>> predicate);
    }
}
