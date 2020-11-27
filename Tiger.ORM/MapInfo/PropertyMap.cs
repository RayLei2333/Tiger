using System.Reflection;
using Tiger.ORM.ModelConfiguration.Attr;

namespace Tiger.ORM.MapInfo
{
    public class PropertyMap
    {
        public string Name { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public KeyType KeyType { get; set; }

    }
}
