using System;

namespace Tiger.ORM.ModelConfiguration
{
    /// <summary>
    /// 主键属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : ModelConfigAttribute
    {

        internal KeyType KeyType { get; set; }

        public KeyAttribute() { }

        /// <summary>
        /// 主键
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="keyType">主键类型：默认数据库自增长类型</param>
        public KeyAttribute(string columnName, KeyType keyType = KeyType.Identity)
        {
            this.Name = columnName;
            this.KeyType = keyType;
        }

        /// <summary>
        /// 主键
        /// </summary>
        /// <param name="keyType">主键类型：默认数据库自增长类型</param>
        public KeyAttribute(KeyType keyType = KeyType.Identity)
        {
            this.KeyType = keyType;
        }
    }
}
