using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Tiger.ORM.Expressions;
using Tiger.ORM.Expressions.Query;

namespace Tiger.ORM.Adapter
{
    internal abstract class BaseQueryable : IQueryAdapter
    {
        public string GenerateSQL<T>(QueryEntity entity, out DynamicParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
