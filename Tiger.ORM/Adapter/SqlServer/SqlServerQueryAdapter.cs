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

        public string GenerateSQL<T>(QueryEntity entity)
        {
            DynamicParameters parameters = new DynamicParameters();
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

            //generate query column
            sb.Append(this.SetQueryColunm(entity.QueryColumn));
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
        private string SetQueryColunm(IEnumerable<PropertyInfo> queryColumns)
        {
            int queryCount = queryColumns.Count();
            if (queryColumns == null || queryCount <= 0)
                return " *";
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

        private string SetWhere(IEnumerable<LambdaWhereEntity> queryCondition, DynamicParameters parameters)
        {
            if (queryCondition == null || queryCondition.Count() <= 0)
                return "";
            StringBuilder sb = new StringBuilder();
            sb.Append(" WHERE");
            foreach (var item in queryCondition)
            {
                if (item.Property == null)
                {
                    sb.Append($" {item.Operation} ");
                    continue;
                }

                string columnName = Relations.GetColumnNameIncludeKey(item.Property);
                if (item.Operation.ToLower() != "like")
                {
                    string paraName = $"@{columnName}";
                    sb.Append($"{_leftPlaceholder}{columnName}{_rightPlaceholder}{item.Operation}{paraName}");
                    parameters.Add(paraName, item.Value);
                }
                else
                    sb.Append($"{_leftPlaceholder}{columnName}{_rightPlaceholder} {item.Operation} {item.Value}");
            }
            return sb.ToString();
        }


        private string PorpetyToColumn(IEnumerable<PropertyInfo> properties, string defaultValue = "", string orderbyColumn = "")
        {
            int propertyCount = properties.Count();
            if (properties == null || propertyCount <= 0)
                return defaultValue;
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

        private string SetOrderBy(IEnumerable<PropertyInfo> orderby, out bool hasOrderBy, string byKey = "ASC")
        {
            hasOrderBy = false;
            int orderbyCount = orderby.Count();
            if (orderby == null || orderbyCount <= 0)
                return "";
            hasOrderBy = true;
            StringBuilder sb = new StringBuilder();
            int index = 0,
                count = orderbyCount - 1;
            foreach (var item in orderby)
            {
                string columnName = Relations.GetColumnName(item);
                sb.Append($"{columnName} {byKey}");
                if (index < count)
                    sb.Append(",");
                index++;
            }
            return sb.ToString();
        }


        private string GroupBy(IEnumerable<PropertyInfo> groupby)
        {
            int groupbyCount = groupby.Count();
            if (groupby == null || groupbyCount <= 0)
                return "";
            StringBuilder sb = new StringBuilder();
            int index = 0,
               count = groupbyCount - 1;
            foreach (var item in groupby)
            {
                string columnName = Relations.GetColumnName(item);
                sb.Append($"{columnName}");
                if (index < count)
                    sb.Append(",");
                index++;
            }

            return sb.ToString();
        }
    }
}
