using System;

namespace Tiger.ORM.ModelConfiguration
{
    /// <summary>
    /// map property to database in table primary key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : BaseModelConfigAttribute
    {
        /// <summary>
        /// primary key data type.
        /// </summary>
        public KeyType KeyType { get; set; }

        public KeyAttribute() { }

        /// <summary>
        /// 默认主键生成规则为“数据库自增长”
        /// </summary>
        /// <param name="keyName"></param>
        public KeyAttribute(string keyName)
        {
            this.Name = keyName;
            this.KeyType = KeyType.Identity;
        }

        /// <summary>
        /// 默认主键列名为属性名称
        /// </summary>
        /// <param name="keyType"></param>
        public KeyAttribute(KeyType keyType)
        {
            this.KeyType = keyType;
        }

        /// <summary>
        /// 自定义主键列名和主键生成类型
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="keyType"></param>
        public KeyAttribute(string keyName,KeyType keyType):this(keyName)
        {
            this.KeyType = keyType;
        }
    }


    public enum KeyType
    {
        Identity = 1,
        GUID = 2,
        AutoGUID = 3
    }
}
