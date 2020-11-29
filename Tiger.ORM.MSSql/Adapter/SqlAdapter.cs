using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tiger.ORM.Adapter;
using Tiger.ORM.Exceptions;
using Tiger.ORM.Expressions;
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

        #region "Delete"
        public string Delete<T>(object key, DynamicParameters parameters)
        {
            Type typeofClass = typeof(T);

            PropertyMap keyMap = TigerRelationMap.GetKey(typeofClass);
            if (keyMap == null)
                throw new TigerORMException($"{typeofClass.FullName}未配置主键，无法执行.");

            string tableName = TigerRelationMap.GetTableName(typeofClass);

            if (parameters == null)
                parameters = new DynamicParameters();

            string sql = $"DELETE FROM {tableName} WHERE {keyMap.Name}=@{keyMap.Name}";
            parameters.Add($"@{keyMap.Name}", key);

            return sql;
        }

        public string Delete<T>(IEnumerable<LambdaProperty> properties, DynamicParameters parameters)
        {
            Type typeofClass = typeof(T);
            string tableName = TigerRelationMap.GetTableName(typeofClass);
            PropertyMap key = TigerRelationMap.GetKey(typeofClass);
            List<PropertyMap> columns = TigerRelationMap.GetColumns(typeofClass).ToList();
            if (key != null)
                columns.Add(key);
            if (parameters == null)
                parameters = new DynamicParameters();

            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (var item in properties)
            {
                if (item.Property == null)
                {
                    sb.Append($" {item.Operation} ");
                    continue;
                }

                PropertyMap map = columns.Where(t => t.PropertyInfo == item.Property).FirstOrDefault();
                if (map == null)
                    throw new TigerORMException($"未找到属性{item.Property.Name}的列映射.");

                sb.Append($"{this.LeftPlaceholder}{map.Name}{this.RightPlaceholder}{item.Operation}@{map.Name + index}");
                parameters.Add($"@{map.Name + index}", item.Value);
                index++;
            }

            string sql = $"DELETE FROM {tableName}";
            if (properties.Count() > 0)
                sql += $" WHERE {sb.ToString()}";
            return sql;
        }
        #endregion


        public string Update(object entity, DynamicParameters parameters)
        {
            Type typeofClass = entity.GetType();
            string tableName = TigerRelationMap.GetTableName(typeofClass);
            PropertyMap key = TigerRelationMap.GetKey(typeofClass);
            if (key == null)
                throw new TigerORMException($"{typeofClass.FullName}未配置主键，无法执行.");
            IEnumerable<PropertyMap> columns = TigerRelationMap.GetColumns(typeofClass);
            if (parameters == null)
                parameters = new DynamicParameters();


            object keyValue = key.PropertyInfo.GetValue(entity);
            if (keyValue == null && (key.KeyType == KeyType.GUID || key.KeyType == KeyType.AutoGUID))
                throw new TigerORMException($"{typeofClass.FullName}主键值为空");
            if (key.KeyType == KeyType.Identity && Convert.ToInt32(keyValue) == 0)
                throw new TigerORMException($"{typeofClass.FullName}主键值为0");

            string where = $"{key.Name}=@{key.Name}";
            parameters.Add($"@{key.Name}", keyValue);


            List<string> setList = new List<string>();
            foreach (var item in columns)
            {
                string set = $"{this.LeftPlaceholder}{item.Name}{this.RightPlaceholder}=@{item.Name}";
                parameters.Add($"@{item.Name}", item.PropertyInfo.GetValue(entity));
            }

            string sql = $"UPDATE {tableName} SET {string.Join(",", setList)} WHERE {where};";
            return sql;
        }
    }
}
