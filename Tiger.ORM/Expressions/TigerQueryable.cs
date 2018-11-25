using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Tiger.ORM.Adapter;
using Tiger.ORM.Expressions.Query;
using Tiger.ORM.Utilities;
using Dapper;

namespace Tiger.ORM.Expressions
{
    public class TigerQueryable<T> : ITigerQueryable<T>
    {

        private static readonly IQueryAdapter _defaultAdapter = new SqlServerQueryAdapter();
        private static readonly Dictionary<string, IQueryAdapter> _queryAdapter = new Dictionary<string, IQueryAdapter>()
        {
            ["sqlconnection"] = new SqlServerQueryAdapter(),
            ["mysqlconnection"] = null
        };

        private IDbConnection _connection { get; set; }

        private IDbTransaction _transaction { get; set; }

        private QueryEntity _queryEntity { get; set; }

        public TigerQueryable()
        {
            this._queryEntity = new QueryEntity();
        }
        
        public ITigerQueryable<T> Select(Expression<Func<T, object>> selector)
        {
            if (this._queryEntity.QueryColumn == null)
                this._queryEntity.QueryColumn = new List<PropertyInfo>();
            this._queryEntity.QueryColumn.AddRange(this.GetExperssionPropety(selector));
            return this;
        }

        public ITigerQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (this._queryEntity.QueryCondition == null)
                this._queryEntity.QueryCondition = new List<LambdaWhereEntity>();
            ExpressionAnalysis analysis = new ExpressionAnalysis();
            MemberType memberType = MemberType.None;
            analysis.Analysis(predicate, ref memberType);
            this._queryEntity.QueryCondition.AddRange(analysis.WhereEntities);
            return this;
        }

        public ITigerQueryable<T> OrderBy(Expression<Func<T, object>> keySelector)
        {
            if (this._queryEntity.Orderby == null)
                this._queryEntity.Orderby = new List<PropertyInfo>();
            this._queryEntity.Orderby.AddRange(this.GetExperssionPropety(keySelector));
            return this;
        }

        public ITigerQueryable<T> OrderByDescending(Expression<Func<T, object>> keySelector)
        {
            if (this._queryEntity.OrderbyDesc == null)
                this._queryEntity.OrderbyDesc = new List<PropertyInfo>();
            this._queryEntity.OrderbyDesc.AddRange(this.GetExperssionPropety(keySelector));
            return this;
        }

        public ITigerQueryable<T> GroupBy(Expression<Func<T, object>> keySelector)
        {
            if (this._queryEntity.Groupby == null)
                this._queryEntity.Groupby = new List<PropertyInfo>();
            this._queryEntity.Groupby.AddRange(this.GetExperssionPropety(keySelector));
            return this;
        }

        //left join\right join\inner join\

        #region 执行层
        public T FirstOrDefault()
        {
            return default(T);
        }

        public int Max()
        {
            return -1;
        }

        public int Min()
        {
            return -1;
        }

        public int Count()
        {
            return -1;
        }

        public int Sum()
        {
            return -1;
        }

        public int Avg()
        {
            return -1;
        }


        public IEnumerable<T> AsEnumerable()
        {
            IQueryAdapter adapter = this.GetAdapter();
            string sql = "";
            IEnumerable<T> result = this._connection.Query<T>(sql,null,this._transaction);
            return result;
        }

        public List<T> ToList()
        {
            List<T> enumerable = this.AsEnumerable().ToList();
            return enumerable;
        }
        #endregion

        private List<PropertyInfo> GetExperssionPropety(Expression<Func<T, object>> expression)
        {
            List<PropertyInfo> propertyList = new List<PropertyInfo>();
            MemberInitExpression member = expression.Body as MemberInitExpression;
            foreach (var item in member.Bindings)
            {
                MemberAssignment assignment = item as MemberAssignment;
                PropertyInfo property = (PropertyInfo)assignment.Member;
                propertyList.Add(property);
            }
            return propertyList;
        }

        private IQueryAdapter GetAdapter()
        {
            string dbType = this._connection.GetType().Name.ToLower();
            if (!_queryAdapter.ContainsKey(dbType))
                return _defaultAdapter;
            return _queryAdapter[dbType];
        }
    }
}
