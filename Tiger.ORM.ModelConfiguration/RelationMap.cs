using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.ORM.ModelConfiguration
{
    public class RelationMap : Relations
    {
        internal Type TypeOfClass { get; set; }

        private RuntimeTypeHandle TypeHandel { get; set; }

        public RelationMap(Type typeOfClass)
        {
            this.TypeOfClass = typeOfClass;
            this.TypeHandel = this.TypeOfClass.TypeHandle;
            if (!_typeTableName.ContainsKey(this.TypeHandel))
                base.ObjectMap(this.TypeOfClass);
        }

        //获取表名
        public string GetTable()
        {
            _typeTableName.TryGetValue(this.TypeHandel, out string name);
            return name;
        }

        //获取属性主键属性
        public PropertyInfo GetKeyProperty()
        {
            _typeKey.TryGetValue(this.TypeHandel, out PropertyInfo key);
            return key;
        }

        //获取主键名称
        public string GetKeyName()
        {
            _typeKey.TryGetValue(this.TypeHandel, out PropertyInfo key);
            KeyAttribute keyAttr = key.GetCustomAttribute<KeyAttribute>();
            if (keyAttr == null || string.IsNullOrEmpty(keyAttr.Name))
                return key.Name;
            return keyAttr.Name;
        }

        //获取所有列
        public IEnumerable<PropertyInfo> GetColumns()
        {
            _typeColumn.TryGetValue(this.TypeHandel, out IEnumerable<PropertyInfo> column);
            return column;
        }

        //获取列名
        public string GetColumn(PropertyInfo column)
        {
            ColumnAttribute columnAttr = column.GetCustomAttribute<ColumnAttribute>();
            if (columnAttr == null || string.IsNullOrEmpty(columnAttr.Name))
                return column.Name;
            return columnAttr.Name;
        }
    }
}
