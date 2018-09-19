# TextFileDB - Under development
A lightweight library for setting up and running a textfile database.

The project TextFileDb is the heart of this repository, the rest is just demo
code i have set up to test the library.

# Usage
The library has a class TextDbSchema that you inherit from and in there you set up
your table models with help of different classes.

To see example of how this works you can look at the demo file DbContext file in the 
DataLibrary. There is also a generic repository and interface to inherit from in the 
"Repositories" and "Interfaces" folders.

You can then new up a DbContext and get data from your tables with help of extension 
methods in the TextFileDb library.

You can use simple relationships. A column can hold relationship to one or many.
When you delete a row in the database it will delete the reletionships of that entity
with that id in the other tables also.

I built in functionality to parse some **really basic** sql statements to the database.
You can parse a **Select From Where** statement and get the result back in a DataSet.

Examples: 
This will give you a DataSet with the columns Id, FirstName and LastName that matches the condition.
```SQL
Select [Id], [FirstName], [LastName] From [PersonTbl] Where [FirstName] == 'Robert' && [LastName] != 'Lundgren'
```

This will give you a DataSet with all the columns that matches the condition.
```SQL
Select * From [PersonTbl] Where [FirstName] == 'Robert' && [LastName] != 'Lundgren'
```

The library creates a json file over the database schema, but this file looks a little bit
wonky, will have to look more in to this, never worked with json before.

The library also creates a TextDbInfo.tdb that contains the current primary key for each table
and info about the columns on the tables.

It looks a ittle bit like this:
```
<![DbInfo]>
    <Tables Count=3>
        [PersonsTbl] [PrimaryKeyCount=16] (DataLibrary.Entities.Person)
            <Columns Count=6>
                [Id] (IntType)
                [FirstName] (StringType)
                [LastName] (StringType)
                [EmailAddress] (StringType)
                [CellphoneNumber] (StringType)
                [Prize] (MultipleRelationships)
            </Columns>
        [PrizesTbl] [PrimaryKeyCount=10] (DataLibrary.Entities.Prize)
            <Columns Count=5>
                [Id] (IntType)
                [PlaceNumber] (IntType)
                [PlaceName] (StringType)
                [PrizeAmount] (DecimalType)
                [PrizePercentage] (DoubleType)
            </Columns>
        [TeamsTbl] [PrimaryKeyCount=5] (DataLibrary.Entities.Team)
            <Columns Count=3>
                [Id] (IntType)
                [TeamMembers] (MultipleRelationships)
                [TeamName] (StringType)
            </Columns>
    </Tables>
</[DbInfo]>
```

# Examples
- A table setup in your class that inherits from TextDbSchema
```C#
public IDbTableSet PersonsTbl
{
    get
    {
        IReadOnlyList<IDbColumn> columns = new List<IDbColumn>
        {
            new DbPrimaryKeyColumn<int>("Id", 0, ColumnDataType.IntType),
            new DbColumn("FirstName", 1, ColumnDataType.StringType),
            new DbColumn("LastName", 2, ColumnDataType.StringType),
            new DbColumn("EmailAddress", 3, ColumnDataType.StringType),
            new DbColumn("CellphoneNumber", 4, ColumnDataType.StringType),
            new DbRelationshipColumn("Prize", 5, ColumnDataType.MultipleRelationships, typeof(Prize), "PrizesTbl")
        };
        string dbTextFile = "PersonModels.csv";
        string tableName = "PersonsTbl";

        var tblSet = new DbTableSet<Person>(columns, dbTextFile, tableName);

        return tblSet;
    }
}
```

- New up of your class that inherits from TextDbSchema, in this case DbContext
```C#
var db = new DbContext();
```


- Add entity, returns entity with new id \=\> 
```c#
db.PersonsTbl.Add(entity);
```


- Read entity by id \=\> 
```c#
db.PersonsTbl.Read<Person>(1);
```

- Update entity, returns entity \=\> 
```c#
db.PersonsTbl.Update(entity);
```

- Delete entity, relations to this entity will be deleted as well =\> 
```c#
db.PersonTbl.Delete(entity);
```
- List all =\> 
```c#
db.PersonsTbl.List<Person>();
```
- AddEntities, add a list\<entity\> to the database =\> 
```c#
db.PersonsTbl.AddEntities(entityList);
```

# Bugs
Yes. Probably alot at the moment. Have not cared to much about error handling and 
user input controls yet.

# Credits
I have to give some credits to [**Tim Corey**](https://www.youtube.com/user/IAmTimCorey/featured). I got the idea to make this library watching
one of his videos on youtube where he built an application using text files as a simple database and
i wanted to try to make something like that but more generic, that is somewhat scaleable and reusable
and has a little more functions.

I took the Entity layout, some extension methods and the basic idea and just started from there.

I have learned alot watching his stuff and i want to give thanks to him for making them for us
who really wants to learn C#.

# More info to come...
Feel free to involve yourself in this if you want :)
