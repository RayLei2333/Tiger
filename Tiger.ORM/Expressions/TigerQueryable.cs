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
using Tiger.ORM.Adapter.SqlServer;
using Tiger.ORM.Adapter.MySql;

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
        private static readonly Dictionary<string, ISqlTimeHandle> _timeHandle = new Dictionary<string, ISqlTimeHandle>()
        {
            ["sqlconnection"] = new SqlServerTimeHandle(),
            ["mysqlconnection"] = new MySqlTimeHandle()
        };


        private IDbConnection _connection { get; set; }

        private IDbTransaction _transaction { get; set; }

        private QueryEntity _queryEntity { get; set; }

        public TigerQueryable(IDbConnection connection, IDbTransaction transaction)
        {
            this._connection = connection;
            this._transaction = transaction;
            this._queryEntity = new QueryEntity();
        }

        public ITigerQueryable<T> SelectColumn(Expression<Func<T, object>> selector)
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
            ISqlTimeHandle timeHandle = this.GetTimeHandle();
            ExpressionAnalysis analysis = new ExpressionAnalysis(timeHandle);
            MemberType memberType = MemberType.None;
            analysis.Analysis(predicate, ref memberType);
            this._queryEntity.QueryCondition.AddRange(analysis.WhereEntities);
            foreach (var item in analysis.WhereEntities)
            {
                Console.WriteLine($"Name:{item.Property?.Name}\tValue:{item.Value?.ToString()}");
            }
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
            this._queryEntity.Top = 1;
            IQueryAdapter adapter = this.GetAdapter();
            string sql = adapter.GenerateSQL<T>(this._queryEntity, out DynamicParameters parameters);
            Console.WriteLine(sql);
            T result = this._connection.QueryFirstOrDefault<T>(sql, parameters, this._transaction);
            return result;
        }

        public decimal Max()
        {

            return -1;
        }

        public decimal Min()
        {
            return -1;
        }

        public int Count()
        {
            this._queryEntity.Aggregates = AggregateEnum.Count;
            IQueryAdapter adapter = this.GetAdapter();
            string sql = adapter.GenerateSQL<T>(this._queryEntity, out DynamicParameters parameters);
            Console.WriteLine(sql);
            //this.AddAggregatesParam(null, AggregateEnum.Count);
            //get sql execute
            return -1;
        }

        public decimal Sum()
        {
            //List<PropertyInfo> properties = this.GetExperssionPropety(expression);
            //if (properties.Count > 1)
            //    throw new Exceptions.TigerORMException("目前只支持单列SUM,如需使用多列SUM,请使用SQL语句完成");
            //this.AddAggregatesParam(properties.FirstOrDefault(), AggregateEnum.Count);
            return -1;
        }

        public decimal Avg()
        {
            return -1;
        }


        public IEnumerable<T> AsEnumerable()
        {
            IQueryAdapter adapter = this.GetAdapter();
            string sql = "";
            IEnumerable<T> result = this._connection.Query<T>(sql, null, this._transaction);
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
            //判断是Mmeber还是new
            if (expression.Body is MemberExpression)
            {
                MemberExpression member = expression.Body as MemberExpression;
                PropertyInfo property = (PropertyInfo)member.Member;
                propertyList.Add(property);
            }
            else if (expression.Body is NewExpression)
            {
                NewExpression newExpression = expression.Body as NewExpression;
                foreach (var item in newExpression.Members)
                {
                    PropertyInfo property = (PropertyInfo)item;
                    propertyList.Add(property);
                }
            }
            else if (expression.Body is UnaryExpression)
            {
                UnaryExpression ue = (UnaryExpression)expression.Body;
                //PropertyInfo property = ue.m
                MemberExpression member = (MemberExpression)ue.Operand;
                propertyList.Add(member.Member as PropertyInfo);
                //表达式
            }
            return propertyList;
        }

        private IQueryAdapter GetAdapter()
        {
            return _defaultAdapter;
            //string dbType = this._connection.GetType().Name.ToLower();
            //if (!_queryAdapter.ContainsKey(dbType))
            //    return _defaultAdapter;
            //return _queryAdapter[dbType];
        }

        private ISqlTimeHandle GetTimeHandle()
        {
            return new SqlServerTimeHandle();
            //string dbType = this._connection.GetType().Name.ToLower();
            //if (!_queryAdapter.ContainsKey(dbType))
            //    return _defaultAdapter;
            //return _queryAdapter[dbType];
        }

        //private void AddAggregatesParam(PropertyInfo property, AggregateEnum aggType)
        //{
        //    if (this._queryEntity.Aggregates == null)
        //        this._queryEntity.Aggregates = new AggregateEntity()
        //        {
        //            Property = property,
        //            AggType = aggType
        //        };
        //}
    }
}
