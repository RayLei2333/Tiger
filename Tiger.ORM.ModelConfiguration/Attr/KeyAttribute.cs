using System;

namespace Tiger.ORM.ModelConfiguration.Attr
{
    /// <summary>
    /// 数据库主键映射
    /// 默认主键生成规则为“数据库自增长”
    /// 默认主键名称为“属性名”
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : TigerModelConfigAttribute
    {
        /// <summary>
        /// 数据库主键类型
        /// </summary>
        public KeyType KeyType { get; set; }


        /// <summary>
        /// 默认主键生成规则为“数据库自增长”
        /// 默认主键名称为“属性名”
        /// </summary>
        public KeyAttribute()
        {
            this.KeyType = KeyType.Identity;
        }

        /// <summary>
        /// 设置主键名称
        /// </summary>
        /// <param name="name">数据库列名称</param>
        public KeyAttribute(string name) : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// 设置主键生成规则
        /// </summary>
        /// <param name="keyType">数据库主键规则</param>
        public KeyAttribute(KeyType keyType)
        {
            this.KeyType = keyType;
        }

        /// <summary>
        /// 自定义主键列名和主键生成类型
        /// </summary>
        /// <param name="name">数据库列名称</param>
        /// <param name="keyType">数据库主键规则</param>
        public KeyAttribute(string name, KeyType keyType)
        {
            this.Name = name;
            this.KeyType = keyType;
        }

    }

    public enum KeyType
    {
        /// <summary>
        /// 数据库自增长ID
        /// </summary>
        Identity = 1,

        /// <summary>
        /// GUID
        /// </summary>
        GUID = 2,

        /// <summary>
        /// 自动生成GUID
        /// </summary>
        AutoGUID = 3
    }
}
