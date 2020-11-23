using System;

namespace Tiger.ORM.ModelConfiguration.Attr
{
    /// <summary>
    /// 数据库表映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : TigerModelConfigAttribute
    {
        public TableAttribute() { }

        public TableAttribute(string table)
        {
            this.Name = table;
        }
    }
}
