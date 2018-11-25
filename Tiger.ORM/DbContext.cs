using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;
using Tiger.ORM.Adapter;
using Tiger.ORM.Expressions;
using Tiger.ORM.Utilities;
using Tiger.ORM.Expressions.Query;

namespace Tiger.ORM
{
    public abstract class DbContext : IDisposable
    {
        //private AppConfig _appConfig = new AppConfig();
        private static readonly ISqlAdapter _defaultAdapter = new SqlServerAdapter();
        private static readonly Dictionary<string, ISqlAdapter> _adapterMap = new Dictionary<string, ISqlAdapter>()
        {
            ["sqlconnection"] = new SqlServerAdapter(),
            ["mysqlconnection"] = new MySqlAdapter()
        };

        public IDbConnection Connection { get; set; }

        public IDbTransaction Transaction { get; set; }

        private bool _startTransaction = false;

        public DbContext(DbType db, string connectionString)
        {
            this.Connection = ConnectionFactory.CrateConnection(db, connectionString);
        }

        public DbContext(IDbConnection connection)
        {
            this.Connection = connection;
        }

        public virtual ITigerQueryable<T> Query<T>()
        {
            ITigerQueryable<T> queryable = this.GetQueryable<T>();
            return queryable;
        }

        public virtual IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.Query<T>(sql, param, this.Transaction, buffered, commandTimeout, commandType);
        }

        public virtual IEnumerable<object> Query(Type entityType, string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.Query(entityType, sql, param, this.Transaction, buffered, commandTimeout, commandType);
        }

        public virtual T Get<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.QueryFirstOrDefault<T>(sql, param, this.Transaction, commandTimeout, commandType);
        }

        public virtual long Insert(object entity, int? commandTimeout = null, CommandType? commandType = null)
        {
            ISqlAdapter adapter = this.GetAdapter();
            string sql = adapter.Insert(entity, out DynamicParameters parameters);
            int result = this.Connection.Execute(sql, parameters, this.Transaction, commandTimeout, commandType);
            return result;
        }

        public virtual int Update(object entity, int? commandTimeout = null, CommandType? commandType = null)
        {
            ISqlAdapter adapter = GetAdapter();
            string sql = adapter.Update(entity, out DynamicParameters parameters);
            int result = this.Connection.Execute(sql, parameters, this.Transaction, commandTimeout, commandType);
            return result;
        }

        public virtual ITigerLambda<T> Update<T>(Expression<Func<T, T>> selector)
        {
            ISqlAdapter adapter = this.GetAdapter();
            Dictionary<PropertyInfo, object> updateCollection = new Dictionary<PropertyInfo, object>();
            MemberInitExpression member = selector.Body as MemberInitExpression;
            foreach (var item in member.Bindings)
            {
                MemberAssignment assignment = item as MemberAssignment;
                ConstantExpression constant = assignment.Expression as ConstantExpression;
                PropertyInfo property = (PropertyInfo)assignment.Member;
                updateCollection.Add(property, constant.Value);
            }
            return new UpdateLambda<T>(updateCollection,
                                       this.Connection,
                                       this.Transaction,
                                       adapter);
        }

        public virtual int Delete<T>(object key, int? commandTimeout = null, CommandType? commandType = null)
        {
            ISqlAdapter adapter = GetAdapter();
            string sql = adapter.Delete<T>(key, out DynamicParameters parameters);
            int result = this.Connection.Execute(sql, parameters, this.Transaction, commandTimeout, commandType);
            return result;
        }

        public virtual ITigerLambda<T> Delete<T>()
        {
            ISqlAdapter adapter = GetAdapter();
            return new DeleteLambda<T>(this.Connection,
                                       this.Transaction,
                                       adapter);
        }

        public virtual int Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.Execute(sql, param, this.Transaction, commandTimeout, commandType);
        }

        public virtual T ExecuteScan<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.ExecuteScalar<T>(sql, param, this.Transaction, commandTimeout, commandType);
        }


        public virtual IDbTransaction BeginTransaction(IsolationLevel? il = null)
        {
            if (this.Connection.State == ConnectionState.Closed || this.Connection.State == ConnectionState.Broken)
                this.Connection.Open();
            if (il == null)
                this.Transaction = this.Connection.BeginTransaction();
            else
                this.Transaction = this.Connection.BeginTransaction(il.Value);
            _startTransaction = true;
            return this.Transaction;
        }

        public virtual void Commit()
        {
            if (this.Transaction != null && _startTransaction == true)
            {
                this.Transaction.Commit();
                _startTransaction = false;
            }

        }

        public virtual void Roolback()
        {
            if (this.Transaction != null && _startTransaction == true)
            {
                this.Transaction.Rollback();
                _startTransaction = false;
            }
        }

        private ISqlAdapter GetAdapter()
        {
            string dbType = this.Connection.GetType().Name.ToLower();
            if (!_adapterMap.ContainsKey(dbType))
                return _defaultAdapter;
            return _adapterMap[dbType];
        }

        private ITigerQueryable<T> GetQueryable<T>()
        {
            string dbType = this.Connection.GetType().Name.ToLower();
            if (dbType == "sqlconnection")
                return new SqlServerQueryable<T>(this.Connection, this.Transaction);
            if (dbType == "mysqlconnection")
                return new MySqlQueryable<T>(this.Connection, this.Transaction);
            return new SqlServerQueryable<T>(this.Connection, this.Transaction);
        }

        public virtual void Dispose()
        {
            if (this.Transaction != null)
            {
                //在Dispose 前没有commit 事物，那么在这里执行事物commit，再Dispose 事物
                if (_startTransaction)
                {
                    this.Transaction.Commit();
                    _startTransaction = false;
                }
                this.Transaction.Dispose();
            }
            if (this.Connection != null)
            {
                this.Connection.Close();
                this.Connection.Dispose();
            }
        }
    }
}
