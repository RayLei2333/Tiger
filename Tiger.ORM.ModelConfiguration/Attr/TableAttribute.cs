using System;

namespace Tiger.ORM.ModelConfiguration
{
    /// <summary>
    /// 表名属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : ModelConfigAttribute
    {
        /// <summary>
        /// 表名
        /// 默认ClassName
        /// </summary>
        public TableAttribute() { }


        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="tableName">表名</param>
        public TableAttribute(string tableName)
        {
            this.Name = tableName;
        }
    }
}
