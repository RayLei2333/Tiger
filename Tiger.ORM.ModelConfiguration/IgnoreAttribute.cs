using System;

namespace Tiger.ORM.ModelConfiguration
{
    /// <summary>
    /// not map property to database in table column.
    /// </summary>
    public class IgnoreAttribute : Attribute
    {
        public IgnoreAttribute() { }
    }
}
