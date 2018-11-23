using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.Adapter;
using Tiger.ORM.Utilities;

namespace Tiger.ORM.Expressions
{
    public class UpdateLambda<T> : ITigerLambda<T>
    {
        protected IDbConnection Connection { get; set; }

        protected IDbTransaction Transaction { get; set; }

        protected Dictionary<PropertyInfo, object> UpdateCollection { get; set; }

        protected List<LambdaWhereEntity> UpdateCondition { get; set; }

        protected ISqlAdapter SqlAdapter { get; set; }

        public UpdateLambda(Dictionary<PropertyInfo, object> updateCollection,
                            IDbConnection connection,
                            IDbTransaction transaction,
                            ISqlAdapter adapter)
        {
            this.UpdateCollection = updateCollection;
            this.UpdateCondition = new List<LambdaWhereEntity>();
            this.Connection = connection;
            this.Transaction = transaction;
            this.SqlAdapter = adapter;
        }


        public virtual ITigerLambda<T> Where(Expression<Func<T, bool>> predicate)
        {
            //解析Expression
            ExpressionAnalysis analysis = new ExpressionAnalysis();
            MemberType memberType = MemberType.None;
            analysis.Analysis(predicate, ref memberType);
            this.UpdateCondition.AddRange(analysis.WhereEntities);
            return this;
        }


        public virtual int Execute(int? commandTimeout = null, CommandType? commandType = null)
        {
            //拼接sql
            string sql = this.SqlAdapter.Update<T>(this.UpdateCollection, this.UpdateCondition, out DynamicParameters parameters);
            int result = this.Connection.Execute(sql, parameters, this.Transaction, commandTimeout, commandType);
            return result;
        }
    }
}
