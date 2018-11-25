using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.Adapter;
using Tiger.ORM.Utilities;

namespace Tiger.ORM.Expressions
{
    public class DeleteLambda<T> : ITigerLambda<T>
    {
        protected IDbConnection Connection { get; set; }

        protected IDbTransaction Transaction { get; set; }

        protected List<LambdaWhereEntity> DeleteCondition { get; set; }

        protected ISqlAdapter SqlAdapter { get; set; }

        public DeleteLambda(IDbConnection connection,
                            IDbTransaction transaction,
                            ISqlAdapter adapter)
        {
            this.Connection = connection;
            this.Transaction = transaction;
            this.SqlAdapter = adapter;
        }

        public virtual ITigerLambda<T> Where(Expression<Func<T, bool>> predicate)
        {
            MemberType memberType = MemberType.None;
            ExpressionAnalysis analysis = new ExpressionAnalysis();
            analysis.Analysis(predicate, ref memberType);
            this.DeleteCondition.AddRange(analysis.WhereEntities);
            return this;
        }

        public virtual int Execute(int? commandTimeout = null, CommandType? commandType = null)
        {
            string sql = this.SqlAdapter.Delete<T>(this.DeleteCondition, out DynamicParameters parameters);
            int result = this.Connection.Execute(sql, parameters, this.Transaction, commandTimeout, commandType);
            return result;
        }
    }
}
