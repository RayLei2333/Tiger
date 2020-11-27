using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.ModelConfiguration.Attr;

namespace ConsoleApp1.Model
{
    public class T_WeChat_User
    {
        [Key(KeyType.GUID)]
        public string Id { get; set; }

        public string NickName { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public int Gender { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
