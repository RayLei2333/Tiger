using System;

namespace Tiger.ORM.ModelConfiguration
{
    /// <summary>
    /// map property to database in table column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : BaseModelConfigAttribute
    {
        public ColumnAttribute() { }

        public ColumnAttribute(string columnName)
        {
            this.Name = columnName;
        }
    }
}
