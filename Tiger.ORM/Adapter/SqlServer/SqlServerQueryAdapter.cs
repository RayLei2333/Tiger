using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tiger.ORM.Expressions.Query;
using System.Linq;
using Tiger.ORM.Utilities;
using Dapper;

namespace Tiger.ORM.Adapter
{
    internal class SqlServerQueryAdapter : IQueryAdapter
    {
        private readonly string _leftPlaceholder = "[";
        private readonly string _rightPlaceholder = "]";

        public string GenerateSQL<T>(QueryEntity entity, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            Type entityType = typeof(T);
            string tableName = Relations.GetTableName(entityType);
            List<PropertyInfo> columns = Relations.GetColumns(entityType).ToList();
            PropertyInfo key = Relations.GetKey(entityType);
            columns.Add(key);
            //SQL:SELECT TOP 1 * FROM table where ... ORDER BY ... GROUP BY ....
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT");

            if (entity.Top != -1)
                sb.Append($" TOP {entity.Top}");
            //generate agg and query column


            //generate query column
            sb.Append(this.SetQueryColunm(entity.QueryColumn,entity.Aggregates));
            // FROM {TABLE}
            sb.Append($" FROM {tableName}");
            //Generate WHERE 
            sb.Append(this.SetWhere(entity.QueryCondition, parameters));

            //Generate order by 
            string orderby = this.PorpetyToColumn(entity.Orderby, null, "ASC");
            string orderbyDesc = this.PorpetyToColumn(entity.OrderbyDesc, null, "DESC");
            if (orderby != null || orderbyDesc != null)
                sb.Append(" ORDER BY ");
            sb.Append(orderby)
              .Append(orderbyDesc);

            string groupby = this.PorpetyToColumn(entity.Groupby, null);
            if (groupby != null)
                sb.Append($" GROUP BY {groupby}");

            return sb.ToString();
        }
        private string SetQueryColunm(IEnumerable<PropertyInfo> queryColumns, AggregateEnum aggregate)
        {
            if ((queryColumns != null && aggregate != AggregateEnum.None) && queryColumns.Count() > 1)
                throw new Exceptions.TigerORMException("目前只支持单列的聚合函数，如需使用多列，请用SQL语句完成");
            if (aggregate != AggregateEnum.None)
            {
                PropertyInfo aggProperty = queryColumns?.FirstOrDefault(); 
                string aggColumn = null;
                if (aggProperty == null)
                    aggColumn = "1";
                else
                    aggColumn = Relations.GetColumnName(aggProperty);
                return $" {aggregate.ToString().ToUpper()}({aggColumn})";
            }
            else
            {
                if (queryColumns == null || queryColumns.Count() <= 0)
                    return " *";
                int queryCount = queryColumns.Count();
                StringBuilder sb = new StringBuilder();
                sb.Append(" ");
                int index = 0,
                    count = queryCount - 1;
                foreach (var item in queryColumns)
                {
                    string columnName = Relations.GetColumnNameIncludeKey(item);
                    sb.Append(columnName);
                    if (index < count)
                        sb.Append(",");
                    index++;
                }

                return sb.ToString();
            }
        }

        private string SetWhere(IEnumerable<LambdaWhereEntity> queryCondition, DynamicParameters parameters)
        {
            if (queryCondition == null || queryCondition.Count() <= 0)
                return "";
            StringBuilder sb = new StringBuilder();
            sb.Append(" WHERE ");
            foreach (var item in queryCondition)
            {
                if (item.Property == null)
                {
                    sb.Append($" {item.Operation} ");
                    continue;
                }

                string columnName = Relations.GetColumnNameIncludeKey(item.Property);
                string newColumnName = null,
                       paraName = null;
                if(item.Property.PropertyType.Name == "DateTime")
                {
                    newColumnName = $"CONVERT(VARCHAR(10),{_leftPlaceholder}{columnName}{_rightPlaceholder},120)";
                    paraName = $"CONVERT(VARCHAR(10),@{columnName},120)";
                }
                else
                {
                    newColumnName = $"{_leftPlaceholder}{columnName}{_rightPlaceholder}";
                    paraName = $"@{columnName}";
                }
                if (item.Operation.ToLower() != "like")
                {
                    sb.Append($"{newColumnName}{item.Operation}{paraName}");
                    parameters.Add($"@{columnName}", item.Value);
                }
                else
                    sb.Append($"{newColumnName} {item.Operation} {item.Value}");
                
            }
            return sb.ToString();
        }

        private string PorpetyToColumn(IEnumerable<PropertyInfo> properties, string defaultValue = "", string orderbyColumn = "")
        {
            if (properties == null || properties.Count() <= 0)
                return defaultValue;
            int propertyCount = properties.Count();
            StringBuilder sb = new StringBuilder();
            int index = 0,
                count = propertyCount - 1;
            foreach (var item in properties)
            {
                string columnName = Relations.GetColumnName(item);
                sb.Append($"{columnName} {orderbyColumn}");
                if (index < count)
                    sb.Append(",");
                index++;
            }
            return sb.ToString();
        }

    }
}
