using System;

namespace Tiger.ORM.ModelConfiguration
{
    /// <summary>
    /// 忽略属性
    /// 生成sql语句中不包含这列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : ModelConfigAttribute
    {
        /// <summary>
        /// 忽略
        /// 默认PropertyInfo.Name
        /// </summary>
        public IgnoreAttribute() { }

        /// <summary>
        /// 忽略
        /// </summary>
        /// <param name="columnName">列名</param>
        public IgnoreAttribute(string columnName)
        {
            this.Name = columnName;
        }
    }
}
