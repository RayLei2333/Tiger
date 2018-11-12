using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace Tiger.ORM.Adapter
{
    public interface ISqlAdapter
    {
        string Inser(object eneity,out DynamicParameters parameters);

        string Update(object eneity, out DynamicParameters parameters);

        string Delete<T>(object key,out DynamicParameters parameters);
    }
}
