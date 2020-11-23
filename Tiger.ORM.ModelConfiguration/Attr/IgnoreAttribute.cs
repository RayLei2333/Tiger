using System;

namespace Tiger.ORM.ModelConfiguration.Attr
{
    /// <summary>
    /// 忽略属性，不向数据库映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        public IgnoreAttribute() { }
    }
}
