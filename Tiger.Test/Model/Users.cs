using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.ModelConfiguration;

namespace Tiger.Test.Model
{
    public class Users
    {
        [Key("ids",KeyType.AutoGUID)]
        public string Id { get; set; }

        public int Height { get; set; }


        public string Name { get; set; }
    }
}
