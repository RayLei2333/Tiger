# Tiger.ORM - a dapper extension

Features
--------
Tiger.ORM is .NET [Dapper](https://github.com/StackExchange/Dapper) extension.More efficient and quick access to the database for more efficient development...<br>
Two databases are now supported(`SqlServer` `MySql`).


How to use it?
--------------
The use of ORM requires 4 steps.
> 1.Add configuration file.<br>
> 2.Defining the data model.<br>
> 3.Defining the data context.<br>
> 4.Perform data operations.<br>

configuration
-------------
```xml
    <!--sql server config-->
    <add name="db" connectionString="Data Source=.;Initial Catalog=testdb;User ID=sa;Password=****;" providerName="System.Data.SqlClient" />
    <!--mysql config-->
    <add name="db" connectionString="Data Source=.;Initial Catalog=testdb;User ID=sa;Password=****;" providerName="MySql.Data.MySqlClient" />
```

Defining data model
-------------------
Example usage:

```csharp
[Table("user")]                         //if class name is table name,can choose to use or not use [TableAttribute]
public class Users
{
    [Key("id", KeyType.AutoGUID)]       //map property to database in table primary key.
    public string Id { get; set; }

    [Ignore]                            //not map property to database in table column.
    public int Height { get; set; }
    
    [Column("UserName")]                //map property to database in table column.
    public string Name { get; set; }

                                        //if property name is column name,Can choose to use or not use [ColumnAttribute]
    public int Gender { get; set; }

    public DateTime CreatTime { get; set; }
}
```

Defining data context
---------------------
Example usage:
```csharp
public class TestContext : DbContext
{
    public TestContext() : base("name or connection")
    {
    }

    public TestContext() : base(new MySqlConnection("connection string"))
    {

    }
}
```