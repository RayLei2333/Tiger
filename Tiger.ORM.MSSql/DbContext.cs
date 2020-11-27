using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.Adapter;
using Tiger.ORM.MapInfo;
using Tiger.ORM.SqlServer.Adapter;
using Tiger.ORM.SqlServer.Utilities;

namespace Tiger.ORM.SqlServer
{
    public abstract class DbContext : TigerDbContext
    {
        //internal ISqlAdapter Adapter { };
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

        
    }
}
