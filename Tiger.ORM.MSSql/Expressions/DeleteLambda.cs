using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.Expressions;

namespace Tiger.ORM.SqlServer.Expressions
{
    public class DeleteLambda<T> : ITigerLambda<T>
    {
        private TigerDbContext _tigerDbContext { get; set; }

        private List<LambdaProperty> _whereList { get; set; }

        private ExpressionAnalysis _expressionAnalysis { get; set; }

        public DeleteLambda(TigerDbContext context)
        {
            this._tigerDbContext = context;
            this._whereList = new List<LambdaProperty>();
            this._expressionAnalysis = new ExpressionAnalysis();
        }

        public int Execute(int? commandTimeout = null, CommandType? commandType = null)
        {
            this._whereList.AddRange(this._expressionAnalysis.LambdaProperties);
            throw new NotImplementedException();
        }

        public ITigerLambda<T> Where(Expression<Func<T, bool>> predicate)
        {
            TigerMemberType memberType = TigerMemberType.None;
            this._expressionAnalysis.Analysis(predicate, ref memberType);
            return this;
        }
    }
}
