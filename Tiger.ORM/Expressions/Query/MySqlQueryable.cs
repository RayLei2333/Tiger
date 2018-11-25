using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Tiger.ORM.Expressions.Query
{
    public class MySqlQueryable<T> : SqlServerQueryable<T>,ITigerQueryable<T>
    {
        public MySqlQueryable(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }
        
    }
}
