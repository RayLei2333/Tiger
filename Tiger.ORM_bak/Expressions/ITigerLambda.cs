using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.ORM.Expressions
{
    public interface ITigerLambda<T>
    {
        ITigerLambda<T> Where(Expression<Func<T, bool>> predicate);
        
        int Execute(int? commandTimeout = null, CommandType? commandType = null);

    }
}
