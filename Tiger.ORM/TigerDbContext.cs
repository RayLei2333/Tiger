using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.Adapter;

namespace Tiger.ORM
{
    public abstract class TigerDbContext : IDisposable
    {

        public IDbConnection Connection { get; set; }

        public IDbTransaction Transaction { get; set; }

        /// <summary>
        /// 批量新增判断条件，默认100条
        /// 当值小于xxx时，批量新增采用sql
        /// 当值大于xxx是，批量新增采用block
        /// </summary>
        public int BatchInsertNumber { get; set; } = 100;

        protected ISqlAdapter Adapter { get; set; }

        private bool StartTransaction { get; set; }

        #region "构造函数"
        public TigerDbContext(IDbConnection connection)
        {
            this.Connection = connection;
        }
        #endregion


        #region "数据库事物"
        public virtual IDbTransaction BeginTransaction(IsolationLevel? il = null)
        {
            this.OpenConnection();
            if (il != null)
                this.Transaction = this.Connection.BeginTransaction();
            else
                this.Transaction = this.Connection.BeginTransaction(il.Value);
            this.StartTransaction = true;
            return this.Transaction;
        }

        public virtual void Commit()
        {
            if (this.Transaction != null && this.StartTransaction)
            {
                this.Transaction.Commit();
                this.StartTransaction = false;
            }
        }

        public virtual void Rollback()
        {
            if (this.Transaction != null && this.StartTransaction)
            {
                this.Transaction.Rollback();
                this.StartTransaction = false;
            }
        }
        #endregion



        protected virtual void OpenConnection()
        {
            if (this.Connection.State == ConnectionState.Closed || this.Connection.State == ConnectionState.Broken)
                this.Connection.Open();
        }

        public void Dispose()
        {
            if (this.Transaction != null)
            {
                //在Dispose 前没有commit 事物，那么在这里执行事物commit，再Dispose 事物
                if (this.StartTransaction)
                {
                    this.Transaction.Commit();
                    this.StartTransaction = false;
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
