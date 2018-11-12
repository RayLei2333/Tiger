using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace Tiger.ORM.Adapter
{
    public interface ISqlAdapter
    {
        string Insert(object entity,out DynamicParameters parameters);

        string Update(object entity, out DynamicParameters parameters);

        string Delete<T>(object key,out DynamicParameters parameters);
    }
}
