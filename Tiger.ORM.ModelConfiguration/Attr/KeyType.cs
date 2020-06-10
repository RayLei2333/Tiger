namespace Tiger.ORM.ModelConfiguration
{
    public enum KeyType
    {
        /// <summary>
        /// 手动赋值
        /// </summary>
        None = 1,

        /// <summary>
        /// 数据库忽略该列
        /// </summary>
        Ignore = 2,

        /// <summary>
        /// 自增长主键
        /// </summary>
        Identity = 3,

        /// <summary>
        /// ORM生成GUID长度36位
        /// </summary>
        AutoGUID = 4,
    }
}
