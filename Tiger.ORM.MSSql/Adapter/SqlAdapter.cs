using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tiger.ORM.Adapter;
using Tiger.ORM.MapInfo;
using Tiger.ORM.ModelConfiguration.Attr;

namespace Tiger.ORM.SqlServer.Adapter
{
    public class SqlAdapter : ISqlAdapter
    {
        protected string LeftPlaceholder { get { return "["; } }

        protected string RightPlaceholder { get { return "]"; } }

        #region "Insert"
        public string Insert(object entity, DynamicParameters parameters)
        {
            string sqlTmpl = "INSERT INTO {0}({1}) VALUES {2}";
            if (parameters == null)
                parameters = new DynamicParameters();
            Type entityType = entity.GetType();
            string tableName = TigerRelationMap.GetTableName(entityType);
            IEnumerable<PropertyMap> columns = TigerRelationMap.GetColumns(entityType);
            PropertyMap key = TigerRelationMap.GetKey(entityType);

            string columnStr = this.GetInsertColumn(key, columns);
            string columnPara = this.GetInsterPara(entity, 0, key, columns, parameters);

            string sql = string.Format(sqlTmpl, tableName, columnStr, "(" + columnPara + ")");
            return sql;
        }

        public string Insert<T>(IEnumerable<T> entities, DynamicParameters parameters)
        {
            if (parameters == null)
                parameters = new DynamicParameters();
            Type entityType = typeof(T);
            string tableName = TigerRelationMap.GetTableName(entityType);
            IEnumerable<PropertyMap> columns = TigerRelationMap.GetColumns(entityType);
            PropertyMap key = TigerRelationMap.GetKey(entityType);

            string columnStr = this.GetInsertColumn(key, columns);

            List<string> allColumnParaStrList = new List<string>();
            int index = 0;
            foreach (var entity in entities)
            {
                string columnPara = this.GetInsterPara(entity, index, key, columns, parameters);
                allColumnParaStrList.Add("(" + columnPara + ")");
                index++;
            }

            string sqlTmpl = "INSERT INTO {0}({1}) VALUES {2}";
            string sql = string.Format(sqlTmpl, tableName, columnStr, string.Join(",", allColumnParaStrList));
            return sql;
        }

        private string GetInsertColumn(PropertyMap key, IEnumerable<PropertyMap> columns)
        {
            List<string> columnList = new List<string>();
            if (key != null && key.KeyType != KeyType.Identity)
                columnList.Add($"{this.LeftPlaceholder}{key.Name}{this.RightPlaceholder}");
            foreach (var item in columns)
                columnList.Add($"{this.LeftPlaceholder}{item.Name}{this.RightPlaceholder}");

            return string.Join(",", columnList);
        }

        //INSERT INTO {TABLE}({C1},{C2}) VALUES ({V1},{V2}),({V1},{V2})
        private string GetInsterPara(object entity, int index, PropertyMap key, IEnumerable<PropertyMap> columns, DynamicParameters parameters)
        {
            List<string> columnParaStrList = new List<string>();
            if (key != null && key.KeyType != KeyType.Identity)
            {
                columnParaStrList.Add($"@{key.Name}{index}");
                object val = key.KeyType == KeyType.GUID ? key.PropertyInfo.GetValue(entity) : Guid.NewGuid().ToString("d");
                parameters.Add($"@{key.Name}{index}", val);
            }
            foreach (var item in columns)
            {
                columnParaStrList.Add($"@{item.Name}{index}");
                object val = item.PropertyInfo.GetValue(entity);
                parameters.Add($"@{item.Name}{index}", val);
            }

            return string.Join(",", columnParaStrList);
        }
        #endregion
    }
}
