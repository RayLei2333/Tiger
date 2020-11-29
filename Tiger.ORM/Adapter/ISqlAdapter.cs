using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.Expressions;

namespace Tiger.ORM.Adapter
{
    public interface ISqlAdapter
    {
        string Insert(object entity, DynamicParameters parameters);

        string Insert<T>(IEnumerable<T> entities, DynamicParameters parameters);

        string Delete<T>(object key, DynamicParameters parameters);

        string Delete<T>(IEnumerable<LambdaProperty> properties, DynamicParameters parameters);

        string Update(object entity, DynamicParameters parameters);
    }
}
