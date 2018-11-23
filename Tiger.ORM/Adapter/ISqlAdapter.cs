using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dapper;
using Tiger.ORM.Utilities;

namespace Tiger.ORM.Adapter
{
    public interface ISqlAdapter
    {
        string Insert(object entity,out DynamicParameters parameters);

        string Update(object entity, out DynamicParameters parameters);

        string Update<T>(Dictionary<PropertyInfo, object> updateCollection, IEnumerable<LambdaWhereEntity> updateCondition, out DynamicParameters parameters);

        string Delete<T>(object key,out DynamicParameters parameters);

        string Delete(object entity, out DynamicParameters parameters);

        string Delete<T>(List<LambdaWhereEntity> deleteCondition, out DynamicParameters parameters);
    }
}
