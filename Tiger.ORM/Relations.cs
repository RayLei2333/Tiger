using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tiger.ORM.ModelConfiguration;
using System.Linq;

namespace Tiger.ORM
{
    /// <summary>
    /// Object Map To DataBase Relation.
    /// </summary>
    public static class Relations
    {
        //Runtime object to database table name;
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> _typeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        //Runtime object properties to database table column name;
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> _typeColumn = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        //Runtime object properties to database primary key; need ues [KeyAttribute]
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo> _typeKey = new ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo>();

        /// <summary>
        /// object relation to db
        /// </summary>
        /// <param name="typeofCalss"></param>
        private static void ObjectMap(Type typeofCalss)
        {
            RuntimeTypeHandle typeHandle = typeofCalss.TypeHandle;
            //add table
            ObjectMapTable(typeHandle, typeofCalss);

            //get all properties
            List<PropertyInfo> propertyInfos = typeofCalss.GetProperties().ToList();
            //get ignore properties
            IEnumerable<PropertyInfo> ignoreProperties = propertyInfos.Where(t => t.GetCustomAttribute<IgnoreAttribute>() != null).AsEnumerable();
            //remove ignore properties
            if (ignoreProperties.Count() > 0)
            {
                foreach (var item in ignoreProperties)
                    propertyInfos.Remove(item);
            }

            //add primary key
            ObjectMapPrimaryKey(typeHandle, typeofCalss, propertyInfos, out PropertyInfo primaryKey);

            //remove key property
            propertyInfos.Remove(primaryKey);

            //add columns
            ObjectMapColumn(typeHandle, typeofCalss, propertyInfos);


        }
        private static void ObjectMapTable(RuntimeTypeHandle runtimeTypeHandle, Type typeofClass)
        {
            //get table name
            string tableName = null;
            TableAttribute tableAttr = typeofClass.GetCustomAttribute<TableAttribute>();
            if (tableAttr == null || string.IsNullOrEmpty(tableAttr.Name))
                tableName = typeofClass.Name;
            else
                tableName = tableAttr.Name;
            //save table
            bool saveResult = _typeTableName.TryAdd(runtimeTypeHandle, tableName);
            if (!saveResult)
                throw new Tiger.ORM.Exceptions.TigerORMException("try add table to ConcurrentDictionary error.");

        }
        private static void ObjectMapPrimaryKey(RuntimeTypeHandle runtimeTypeHandle, Type typeofClass, IEnumerable<PropertyInfo> properties, out PropertyInfo keyProperty)
        {
            //string parimaryKeyName = null;
            keyProperty = properties.Where(t => t.GetCustomAttribute<KeyAttribute>() != null).FirstOrDefault();
            if (keyProperty == null)
                throw new Tiger.ORM.Exceptions.TigerORMException($"{typeofClass.FullName} not define KeyAttribute.");

            bool result = _typeKey.TryAdd(runtimeTypeHandle, keyProperty);
            if (!result)
                throw new Tiger.ORM.Exceptions.TigerORMException("try add key to ConcurrentDictionary error.");

        }
        private static void ObjectMapColumn(RuntimeTypeHandle runtimeTypeHandle, Type typeofClass, IEnumerable<PropertyInfo> properties)
        {
            bool result = _typeColumn.TryAdd(runtimeTypeHandle, properties);
            if (!result)
                throw new Tiger.ORM.Exceptions.TigerORMException("try add column to ConcurrentDictionary error.");
        }


        /// <summary>
        /// get map table name
        /// </summary>
        /// <param name="typeofClass"></param>
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
        /// get map table primary key
        /// </summary>
        /// <param name="typeofClass">Mapping Class</param>
        /// <returns></returns>
        public static PropertyInfo GetKey(Type typeofClass)
        {
            RuntimeTypeHandle typeHandle = typeofClass.TypeHandle;
            if (!_typeKey.ContainsKey(typeHandle))
                ObjectMap(typeofClass);

            _typeKey.TryGetValue(typeHandle, out PropertyInfo key);
            return key;

        }

        /// <summary>
        /// 获取主键在数据中的名字
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetKeyName(PropertyInfo property, out KeyAttribute keyAttribute)
        {
            keyAttribute = property.GetCustomAttribute<KeyAttribute>();
            if (string.IsNullOrEmpty(keyAttribute.Name))
                return property.Name;
            return keyAttribute.Name;
        }

        /// <summary>
        /// get map table all column.
        /// </summary>
        /// <param name="typeofClass"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetColumns(Type typeofClass)
        {
            RuntimeTypeHandle typeHandle = typeofClass.TypeHandle;
            if (!_typeColumn.ContainsKey(typeHandle))
                ObjectMap(typeofClass);
            _typeColumn.TryGetValue(typeHandle, out IEnumerable<PropertyInfo> column);

            return column;
        }

        public static string GetColumnName(PropertyInfo column)
        {
            string columnName = null;
            ColumnAttribute columnAttr = column.GetCustomAttribute<ColumnAttribute>();
            if (columnAttr == null || string.IsNullOrEmpty(columnAttr.Name))
                columnName = column.Name;
            else
                columnName = columnAttr.Name;

            return columnName;
        }

        public static string GetColumnNameIncludeKey(PropertyInfo property)
        {
            KeyAttribute keyAttr = property.GetCustomAttribute<KeyAttribute>();
            if (keyAttr != null)
            {
                if (string.IsNullOrEmpty(keyAttr.Name))
                    return property.Name;
                return keyAttr.Name;
            }
            else
            {
                //column 
                ColumnAttribute columnAttr = property.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr == null || string.IsNullOrEmpty(columnAttr.Name))
                    return property.Name;
                else
                    return columnAttr.Name;
            }
        }
    }
}
