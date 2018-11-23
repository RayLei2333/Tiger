# Tiger.ORM - a dapper extension

Features
--------
Tiger.ORM is .NET [Dapper](https://github.com/StackExchange/Dapper) extension.More efficient and quick access to the database for more efficient development...<br>
Two databases are now supported(`SqlServer` `MySql`).


How to use it?
--------------
The use of ORM requires 4 steps.
> [1.Add configuration file.](#configuration)<br>
> [2.Defining the data model.](#defining-data-model)<br>
> [3.Defining the data context.](#defining-data-context)<br>
> [4.Perform data operations.](#data-operations)<br>

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

Data operations
---------------
Example usage:
```csharp
using (TestContext context = new TestContext())
{
    //insert 
    long inserResult = context.Insert(new Users()
    {
        Gender = 2,
        Name = "Ray",
        CreatTime = DateTime.Now,
        Height = 22
    });

    //update
    int updateResult1 = context.Update(new Users()
    {
        CreatTime = DateTime.Now,
        Gender = 1,
        Height = 25,
        Name = "Ray2",
        Id = "70eaf55c-099c-42d7-bc31-c49a92a29775"  //primary key it is necessary.
    });

    //update lambda
    int updateResult2 = context.Update<Users>(t => new Users
    {
        Name = "Ray",
        Gender = 2
    }).Where(t => t.Id == "70eaf55c-099c-42d7-bc31-c49a92a29775").Execute();

    //delete
    int deleteResult1 = context.Delete<Users>("70eaf55c-099c-42d7-bc31-c49a92a29775");

    //delete lambda
    int deleteResult2 = context.Delete<Users>()
                               .Where(t => t.Name == "Ray").Execute();
}
```