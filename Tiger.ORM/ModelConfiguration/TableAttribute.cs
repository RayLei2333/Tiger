using System;
using Tiger.ORM.Exceptions;

namespace Tiger.ORM.ModelConfiguration
{
    /// <summary>
    /// map class to database table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : BaseModelConfigAttribute
    {
        public TableAttribute() { }
        
        public TableAttribute(string table)
        {
            if (string.IsNullOrEmpty(table))
                throw new TigerORMException("configuration table name is not null.");
            this.Name = table;
        }
    }
}
