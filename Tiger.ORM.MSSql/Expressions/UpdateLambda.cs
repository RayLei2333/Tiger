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
    internal class UpdateLambda<T> : ITigerUpdateLambda<T>
    {
        private TigerDbContext _dbContext { get; set; }

        private ISqlAdapter _sqlAdapter { get; set; }

        private List<LambdaProperty> _sqlSetList { get; set; }

        private List<LambdaProperty> _sqlWhereList { get; set; }

        public UpdateLambda(TigerDbContext dbContext, ISqlAdapter adapter)
        {
            this._dbContext = dbContext;
            this._sqlAdapter = adapter;
            this._sqlSetList = new List<LambdaProperty>();
            this._sqlWhereList = new List<LambdaProperty>();
        }

        public ITigerUpdateLambda<T> Set(Expression<Func<T, T>> predicate)
        {
            ExpressionAnalysis analysis = new ExpressionAnalysis();
            TigerMemberType memberType = TigerMemberType.None;
            analysis.Analysis(predicate, ref memberType);
            this._sqlSetList.AddRange(analysis.LambdaProperties);

            return this;
        }

        public ITigerLambda<T> Where(Expression<Func<T, bool>> predicate)
        {
            ExpressionAnalysis analysis = new ExpressionAnalysis();
            TigerMemberType memberType = TigerMemberType.None;
            analysis.Analysis(predicate, ref memberType);
            this._sqlWhereList.AddRange(analysis.LambdaProperties);

            return this;
        }

        public int Execute(int? commandTimeout = null, CommandType? commandType = null)
        {
            DynamicParameters parameters = null;
            string sql = this._sqlAdapter.Update<T>(this._sqlSetList, this._sqlWhereList, parameters);
            int result = this._dbContext.Connection.Execute(sql, parameters, this._dbContext.Transaction, commandTimeout, commandType);
            return result;
        }
    }
}
