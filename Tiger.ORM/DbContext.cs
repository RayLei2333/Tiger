using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using Tiger.ORM.Adapter;
using Tiger.ORM.Utilities;

namespace Tiger.ORM
{
    public abstract class DbContext : IDisposable
    {
        private AppConfig _appConfig = new AppConfig();
        private static readonly ISqlAdapter _defaultAdapter = new SqlServerAdapter();
        private static readonly Dictionary<string, ISqlAdapter> _adapterMap = new Dictionary<string, ISqlAdapter>()
        {
            ["sqlconnection"] = new SqlServerAdapter(),
            ["mysqlconnection"] = new MySqlAdapter()
        };

        public IDbConnection Connection { get; set; }

        public IDbTransaction Transaction { get; set; }

        //public DbContext()
        //{

        //}

        public DbContext(string nameOrConnectionString)
        {
            _appConfig.Initialize(nameOrConnectionString);
            Connection = ConnectionFactory.CreateConnection(_appConfig);

        }

        //public DbContext(string connStr, DbProvider provider)
        //{

        //}



        public virtual IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
        }

        public virtual IEnumerable<object> Query(Type entityType, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.Query(entityType, sql, param, transaction, buffered, commandTimeout, commandType);
        }

        public virtual T Get<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public virtual long Insert(object entity, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            ISqlAdapter adapter = this.GetAdapter();
            string sql = adapter.Insert(entity, out DynamicParameters parameters);
            int result = this.Connection.Execute(sql, parameters, transaction, commandTimeout, commandType);
            return result;
        }

        public virtual int Update(object entity, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            ISqlAdapter adapter = GetAdapter();
            string sql = adapter.Update(entity, out DynamicParameters parameters);
            int result = this.Connection.Execute(sql, parameters, transaction, commandTimeout, commandType);
            return result;
        }

        public virtual int Delete<T>(object key, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            ISqlAdapter adapter = GetAdapter();
            string sql = adapter.Delete<T>(key, out DynamicParameters parameters);
            int result = this.Connection.Execute(sql, parameters, transaction, commandTimeout, commandType);
            return result;
        }

        public virtual int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.Execute(sql, param, transaction, commandTimeout, commandType);
        }

        public virtual T ExecuteScan<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
        }


        public virtual IDbTransaction BeginTransaction()
        {
            if (this.Connection.State == ConnectionState.Closed || this.Connection.State == ConnectionState.Broken)
                this.Connection.Open();
            this.Transaction = this.Connection.BeginTransaction();
            return this.Transaction;
        }


        private ISqlAdapter GetAdapter()
        {
            string dbType = this.Connection.GetType().Name.ToLower();
            if (_adapterMap.ContainsKey(dbType))
                return _defaultAdapter;
            return _adapterMap[dbType];
        }

        public virtual void Dispose()
        {
            if (this.Transaction != null)
                this.Transaction.Dispose();
            if (this.Connection != null)
            {
                this.Connection.Close();
                this.Connection.Dispose();
            }
        }
    }
}
