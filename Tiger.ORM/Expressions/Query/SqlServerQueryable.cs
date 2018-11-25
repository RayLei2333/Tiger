using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Tiger.ORM.Utilities;
using System.Reflection;

namespace Tiger.ORM.Expressions.Query
{
    public class SqlServerQueryable<T> : ITigerQueryable<T>
    {
        private IDbConnection _connection { get; set; }

        private IDbTransaction _transaction { get; set; }

        private List<LambdaWhereEntity> _queryCondition { get; set; }   //where

        private List<PropertyInfo> _queryColumn { get; set; }

        public SqlServerQueryable() { }

        public SqlServerQueryable(IDbConnection connection, IDbTransaction transaction)
        {
            this._connection = connection;
            this._transaction = transaction;
            //this._queryCondition = new List<LambdaWhereEntity>();
        }

        //FirstOrDefault
        //Select
        //order by\group by 
        //Max\Min\Avg\Sum\Count
        public ITigerQueryable<T> Select(Expression<Func<T, object>> selector)
        {
            if (this._queryColumn == null)
                this._queryColumn = new List<PropertyInfo>();
            return this;
        }




        public ITigerQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (this._queryCondition == null)
                this._queryCondition = new List<LambdaWhereEntity>();
            ExpressionAnalysis analysis = new ExpressionAnalysis();
            MemberType memberType = MemberType.None;
            analysis.Analysis(predicate, ref memberType);
            this._queryCondition.AddRange(analysis.WhereEntities);
            return this;
        }


        public IEnumerable<T> AsEnumerable()
        {
            throw new NotImplementedException();
        }

        public List<T> ToList()
        {
            List<T> result = this.AsEnumerable().ToList();
            return result;
        }
    }
}
