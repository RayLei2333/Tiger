using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Tiger.ORM.ModelConfiguration.Attr;

namespace Tiger.ORM.MapInfo
{
    public class TigerRelationMap
    {
        //类与表关系
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> _typeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        //属性与列关系
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyMap>> _typeColumn = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyMap>>();

        //属性与主键关系
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, PropertyMap> _typeKey = new ConcurrentDictionary<RuntimeTypeHandle, PropertyMap>();


        /// <summary>
        /// 对象整体映射
        /// </summary>
        /// <param name="typeofCalss">Mapping Class</param>
        private static void ObjectMap(Type typeofCalss)
        {
            RuntimeTypeHandle typeHandle = typeofCalss.TypeHandle;
            ObjectMapTable(typeHandle, typeofCalss);
            IEnumerable<PropertyInfo> propertyInfos = typeofCalss.GetProperties();
            ObjectMapColumn(typeHandle, propertyInfos);
        }

        /// <summary>
        /// 对象映射表名
        /// </summary>
        /// <param name="runtimeTypeHandle">运行时类型</param>
        /// <param name="typeofClass">Mapping Class</param>
        private static void ObjectMapTable(RuntimeTypeHandle runtimeTypeHandle, Type typeofClass)
        {
            string tableName = null;
            TableAttribute tableAttr = typeofClass.GetCustomAttribute<TableAttribute>();
            if (tableAttr == null || string.IsNullOrEmpty(tableAttr.Name))
                tableName = typeofClass.Name;
            else
                tableName = tableAttr.Name;

            //save table
            _typeTableName.TryAdd(runtimeTypeHandle, tableName);
        }

        /// <summary>
        /// 对象映射主键和列名
        /// </summary>
        /// <param name="runtimeTypeHandle">运行时类型</param>
        /// <param name="properties">Mapping Class</param>
        private static void ObjectMapColumn(RuntimeTypeHandle runtimeTypeHandle, IEnumerable<PropertyInfo> properties)
        {
            List<PropertyMap> columnList = new List<PropertyMap>();
            PropertyMap keyMap = null;
            foreach (PropertyInfo item in properties)
            {
                //判断是否忽略属性
                IgnoreAttribute ignore = item.GetCustomAttribute<IgnoreAttribute>();
                if (ignore != null)
                    continue;

                PropertyMap itemMap = new PropertyMap()
                {
                    PropertyInfo = item,
                };
                //获取主键定义
                TigerModelConfigAttribute attr = item.GetCustomAttribute<KeyAttribute>();
                //如果没有主键则去获取列
                if (attr == null)
                {
                    attr = item.GetCustomAttribute<ColumnAttribute>();
                }
                else
                {
                    KeyAttribute key = (KeyAttribute)attr;
                    itemMap.Name = string.IsNullOrEmpty(key.Name) ? item.Name : key.Name;
                    itemMap.KeyType = key.KeyType;
                    keyMap = itemMap;
                    continue;
                }

                itemMap.Name = (attr != null && !string.IsNullOrEmpty(attr.Name) ? attr.Name : item.Name);
                columnList.Add(itemMap);
            }
            _typeColumn.TryAdd(runtimeTypeHandle, columnList);
            _typeKey.TryAdd(runtimeTypeHandle, keyMap);
        }


        /// <summary>
        /// 获取对象映射表名称
        /// </summary>
        /// <param name="typeofClass">Mapping Class</param>
        /// <returns></returns>
        public static string GetTableName(Type typeofClass)
        {
            RuntimeTypeHandle typeHandle = typeofClass.TypeHandle;
            if (!_typeTableName.ContainsKey(typeHandle))
                ObjectMap(typeofClass);
            _typeTableName.TryGetValue(typeHandle, out string tableName);
            return tableName;
        }

        /// <summary>
        /// 获取对象映射主键
        /// </summary>
        /// <param name="typeofClass">Mapping Class</param>
        /// <returns></returns>
        public static PropertyMap GetKey(Type typeofClass)
        {
            RuntimeTypeHandle typeHandle = typeofClass.TypeHandle;
            if (!_typeKey.ContainsKey(typeHandle))
                ObjectMap(typeofClass);

            _typeKey.TryGetValue(typeHandle, out PropertyMap key);
            return key;
        }

        /// <summary>
        /// 获取对象映射主键
        /// </summary>
        /// <param name="typeofClass">Mapping Class</param>
        /// <returns></returns>
        public static IEnumerable<PropertyMap> GetColumns(Type typeofClass)
        {
            RuntimeTypeHandle typeHandle = typeofClass.TypeHandle;
            if (!_typeColumn.ContainsKey(typeHandle))
                ObjectMap(typeofClass);
            _typeColumn.TryGetValue(typeHandle, out IEnumerable<PropertyMap> column);

            return column;
        }
    }
}
