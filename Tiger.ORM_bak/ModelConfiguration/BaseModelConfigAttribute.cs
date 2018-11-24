using System;

namespace Tiger.ORM.ModelConfiguration
{
    /// <summary>
    /// Basic Model Configuration
    /// </summary>
    public class BaseModelConfigAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
