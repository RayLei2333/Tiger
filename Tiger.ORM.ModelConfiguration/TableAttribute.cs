using System;

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
            this.Name = table;
        }
    }
}
