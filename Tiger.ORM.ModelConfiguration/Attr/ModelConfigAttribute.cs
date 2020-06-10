using System;

namespace Tiger.ORM.ModelConfiguration
{
    public class ModelConfigAttribute : Attribute
    {
        internal string Name { get; set; }
    }
}
