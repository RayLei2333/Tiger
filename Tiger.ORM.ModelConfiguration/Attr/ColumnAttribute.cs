using System;

namespace Tiger.ORM.ModelConfiguration
{
    /// <summary>
    /// 列名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : ModelConfigAttribute
    {
        /// <summary>
        /// 列名
        /// 默认PropertyInfo.Name
        /// </summary>
        public ColumnAttribute() { }

        /// <summary>
        /// 列名
        /// </summary>
        /// <param name="columnName">列名</param>
        public ColumnAttribute(string columnName)
        {
            this.Name = columnName;
        }
    }
}
