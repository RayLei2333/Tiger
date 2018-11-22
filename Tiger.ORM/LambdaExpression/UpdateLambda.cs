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

namespace Tiger.ORM.LambdaExpression
{
    public class UpdateLambda<T> : ITigerLambda<T>
    {
        protected AppConfig AppConfig { get; set; } //= new AppConfig();

        protected IDbConnection Connection { get; set; }

        protected IDbTransaction Transaction { get; set; }

        protected Dictionary<PropertyInfo, object> UpdateCollection { get; set; }

        protected Dictionary<PropertyInfo, object> UpdateCondition { get; set; }

        public ISqlAdapter SqlAdapter { get; set; }

        public UpdateLambda(Dictionary<PropertyInfo, object> updateCollection,
                            IDbConnection connection,
                            IDbTransaction transaction,
                            AppConfig appConfig,
                            ISqlAdapter adapter)
        {
            this.UpdateCollection = updateCollection;
            this.UpdateCondition = new Dictionary<PropertyInfo, object>();
            this.Connection = connection;
            this.Transaction = transaction;
            this.AppConfig = appConfig;
            this.SqlAdapter = adapter;
        }


        public ITigerLambda<T> Where(Expression<Func<T, bool>> predicate)
        {
            //解析Expression
            //_whereExpression = predicate;
            return this;
        }


        public int Execute()
        {
            return -1;
        }
    }
}
