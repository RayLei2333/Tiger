using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.Adapter;
using Tiger.ORM.Expressions;

namespace Tiger.ORM.SqlServer.Expressions
{
    public class DeleteLambda<T> : ITigerLambda<T>
    {
        private TigerDbContext _tigerDbContext { get; set; }

        private List<LambdaProperty> _whereList { get; set; }

        private ExpressionAnalysis _expressionAnalysis { get; set; }

        private ISqlAdapter _adapter { get; set; }

        public DeleteLambda(TigerDbContext context, ISqlAdapter adapter)
        {
            this._tigerDbContext = context;
            this._adapter = adapter;
            this._whereList = new List<LambdaProperty>();
            this._expressionAnalysis = new ExpressionAnalysis();
        }

        public int Execute(int? commandTimeout = null, CommandType? commandType = null)
        {
            this._whereList.AddRange(this._expressionAnalysis.LambdaProperties);
            //throw new NotImplementedException();
            DynamicParameters parameters = null;
            string sql = this._adapter.Delete<T>(this._whereList, parameters);
            Console.WriteLine(sql);
            //int result = this._tigerDbContext.Connection.Execute(sql, parameters, this._tigerDbContext.Transaction, commandTimeout, commandType);
            return -1;
        }

        public ITigerLambda<T> Where(Expression<Func<T, bool>> predicate)
        {
            TigerMemberType memberType = TigerMemberType.None;
            this._expressionAnalysis.Analysis(predicate, ref memberType);
            return this;
        }
    }
}
