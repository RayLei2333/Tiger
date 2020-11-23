using System;

namespace Tiger.ORM.ModelConfiguration.Attr
{
    /// <summary>
    /// 数据库列映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : TigerModelConfigAttribute
    {
        public ColumnAttribute() { }

        public ColumnAttribute(string name)
        {
            this.Name = name;
        }
    }
}
