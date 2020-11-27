using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.ORM.Adapter
{
    public interface ISqlAdapter
    {
        string Insert(object entity, DynamicParameters parameters);

        string Insert<T>(IEnumerable<T> entities, DynamicParameters parameters);
    }
}
