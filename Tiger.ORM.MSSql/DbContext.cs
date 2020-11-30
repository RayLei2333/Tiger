using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.Adapter;
using Tiger.ORM.Expressions;
using Tiger.ORM.MapInfo;
using Tiger.ORM.SqlServer.Adapter;
using Tiger.ORM.SqlServer.Expressions;
using Tiger.ORM.SqlServer.Utilities;

namespace Tiger.ORM.SqlServer
{
    public class DbContext : TigerDbContext
    {
        public DbContext(IDbConnection connection) : base(connection)
        {
            this.Adapter = new SqlAdapter();
        }

        public virtual int Insert(object entity, int? commandTimeout = null, CommandType? commandType = null)
        {
            DynamicParameters parameters = null;
            string sql = this.Adapter.Insert(entity, parameters);
            int result = this.Connection.Execute(sql, parameters, this.Transaction, commandTimeout, commandType);
            return result;
        }

        public virtual int BatchInsert<T>(IEnumerable<T> entities, int? commandTimeout = null, CommandType? commandType = null)
        {
            DynamicParameters parameters = null;
            string sql = this.Adapter.Insert(entities, parameters);
            int result = this.Connection.Execute(sql, parameters, this.Transaction, commandTimeout, commandType);
            return result;
        }

        public virtual void BatchInsert<T>(IEnumerable<T> entities, int? bulkcopyTimeout = null, SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default)
        {
            SqlConnection connection = this.Connection as SqlConnection;
            SqlTransaction transaction = this.Transaction as SqlTransaction;
            Type typeOfEntity = typeof(T);
            string tableName = TigerRelationMap.GetTableName(typeOfEntity);
            using (SqlBulkCopy bulk = new SqlBulkCopy(connection, copyOptions, transaction))
            {
                bulk.DestinationTableName = tableName;
                bulk.BatchSize = entities.Count();
                if (bulkcopyTimeout != null && bulkcopyTimeout.HasValue)
                    bulk.BulkCopyTimeout = bulkcopyTimeout.Value;
                DataTable dt = DataTableConvert.ToDataTable<T>(entities);
                bulk.WriteToServer(dt);
            }

        }

        public virtual int Delete<T>(object key, int? commandTimeout = null, CommandType? commandType = null)
        {
            DynamicParameters parameters = null;
            string sql = this.Adapter.Delete<T>(key, parameters);
            int result = this.Connection.Execute(sql, parameters, this.Transaction, commandTimeout, commandType);
            return result;
        }

        public virtual ITigerLambda<T> Delete<T>()
        {
            return new DeleteLambda<T>(this, this.Adapter);
        }

        public virtual int Update(object entity, int? commandTimeout = null, CommandType? commandType = null)
        {
            DynamicParameters parameters = null;
            string sql = this.Adapter.Update(entity, parameters);
            int result = this.Connection.Execute(sql, parameters, this.Transaction, commandTimeout, commandType);
            return result;
        }

        public virtual ITigerUpdateLambda<T> Update<T>()
        {
            return new UpdateLambda<T>(this, this.Adapter);
        }

        public virtual int Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.Execute(sql, param, this.Transaction, commandTimeout, commandType);
        }

        public virtual T ExecuteScan<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.Connection.ExecuteScalar<T>(sql, param, this.Transaction, commandTimeout, commandType);
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
    }
}
