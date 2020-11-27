using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.MapInfo;
using Tiger.ORM.ModelConfiguration.Attr;

namespace Tiger.ORM.SqlServer.Utilities
{
    internal class DataTableConvert
    {

        public static DataTable ToDataTable<T>(IEnumerable<T> list)
        {
            Type typeOfEntity = typeof(T);
            PropertyMap key = TigerRelationMap.GetKey(typeOfEntity);
            IEnumerable<PropertyMap> column = TigerRelationMap.GetColumns(typeOfEntity);
            DataTable dt = new DataTable();
            if (key != null && key.KeyType != KeyType.Identity)
                dt.Columns.Add(key.Name, Nullable.GetUnderlyingType(key.PropertyInfo.PropertyType) ?? key.PropertyInfo.PropertyType);

            foreach (var item in column)
                dt.Columns.Add(item.Name, Nullable.GetUnderlyingType(item.PropertyInfo.PropertyType) ?? item.PropertyInfo.PropertyType);

            foreach (T item in list)
            {
                DataRow row = dt.NewRow();
                if (key != null && key.KeyType != KeyType.Identity)
                    row[key.Name] = key.PropertyInfo.GetValue(item) ?? DBNull.Value;
                foreach (var c in column)
                    row[c.Name] = c.PropertyInfo.GetValue(item) ?? DBNull.Value;

                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
