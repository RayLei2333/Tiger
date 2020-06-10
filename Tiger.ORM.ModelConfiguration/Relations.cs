using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.ModelConfiguration.Exceptions;

namespace Tiger.ORM.ModelConfiguration
{
    public class Relations
    {
        //类到表关系
        protected static readonly ConcurrentDictionary<RuntimeTypeHandle, string> _typeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        //属性到列关系
        protected static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> _typeColumn = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        //属性到主键关系
        protected static readonly ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo> _typeKey = new ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo>();


        //对象映射
        protected void ObjectMap(Type typeOfCalss)
        {
            RuntimeTypeHandle typeHandle = typeOfCalss.TypeHandle;

            List<PropertyInfo> propertyInfos = typeOfCalss.GetProperties().ToList();
            List<PropertyInfo> columnList = new List<PropertyInfo>();
            PropertyInfo keyProperty = null;

            foreach (var item in propertyInfos)
            {
                IgnoreAttribute igAttr = item.GetCustomAttribute<IgnoreAttribute>();
                if (igAttr != null)
                    continue;
                KeyAttribute keyAttr = item.GetCustomAttribute<KeyAttribute>();
                if (keyAttr != null)
                    keyProperty = item;
                else
                    columnList.Add(item);
            }

            //主键映射
            if (keyProperty == null)
                throw new ModelConfigException($"{typeOfCalss.Name}主键列未定义");
            _typeKey.TryAdd(typeHandle, keyProperty);

            //列映射
            _typeColumn.TryAdd(typeHandle, columnList);

            //映射表名
            this.MapTable(typeHandle, typeOfCalss);
        }


        //映射表名
        private void MapTable(RuntimeTypeHandle runtimeTypeHandle, Type typeOfClass)
        {
            string tableName = null;
            TableAttribute tableAttr = typeOfClass.GetCustomAttribute<TableAttribute>();
            if (tableAttr == null || string.IsNullOrEmpty(tableAttr.Name))
                tableName = typeOfClass.Name;
            else
                tableName = tableAttr.Name;

            _typeTableName.TryAdd(runtimeTypeHandle, tableName);
        }
    }
}
