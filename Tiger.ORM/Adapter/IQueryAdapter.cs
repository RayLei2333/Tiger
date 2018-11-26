using System;
using System.Collections.Generic;
using System.Text;
using Tiger.ORM.Expressions.Query;

namespace Tiger.ORM.Adapter
{
    public interface IQueryAdapter
    {

        string GenerateSQL<T>(QueryEntity entity);

    }
}
