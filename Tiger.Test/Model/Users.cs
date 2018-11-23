using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.ModelConfiguration;

namespace Tiger.Test.Model
{
    [Table("user")]                         //if class name is table name,can choose to use or not use [TableAttribute]
    public class Users
    {
        [Key("ids", KeyType.AutoGUID)]      //map property to database in table primary key.
        public string Id { get; set; }

        [Ignore]                            //not map property to database in table column.
        public int Height { get; set; }
        
        [Column("UserName")]                //map property to database in table column.
        public string Name { get; set; }

                                            //if property name is column name,Can choose to use or not use [ColumnAttribute]
        public int Gender { get; set; }

        public DateTime CreatTime { get; set; }
    }
}
