# TextFileDB - Under development
A lightweight library for setting up and running a textfile database.

The project TextDbLibrary is the heart of this repository, the rest is just demo
code i have set up to test the library.


# Usage
The library has a class TextDbSchema that you inherit from and in that class you set up
your table models with help of different classes and interfaces.

To see example of how this is setup look at the demo DbContext file in the 
DataLibrary. There is also a generic repository and interface to inherit from in the 
"Repositories" and "Interfaces" folders.

When you have made your tables and setup the initializing methods for the TextFileDb
it will create the files needed, if they dont exist, and update what needs to be updated
in the TextDbInfo.tdb file.

You can then new up a DbContext and get data from your tables with help of extension 
methods in the TextFileDb library.

You can use simple relationships. A column can hold relationship to one or many.
When you delete a row in the database it will delete the reletionships of that entity
with that id in the other tables also.

It loads the entities relationships automatically with use of the table schema and reflection.

I built in functionality to parse some **really basic** sql statements to the database.
You can parse a **Select From Where** statement and get the result back in a DataSet.


# Files created
All files created wil be cerated in the folder that you choose in .config file.

```xml
<configuration>
  <appSettings>
    <add key="TextDbFolderPath" value="c:\temp\TextDb" />
  </appSettings>
</configuration>
```

## TextDbInfo.tdb
The library creates a TextDbInfo.tdb that contains the current primary key for each table, 
if primary key is of type int otherwise you can look at it as a counter since we dont need
to store string ids like int idas to keep track of them. It also has info about the columns on the tables.

It looks a little bit like this:
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

## PersonsTbl.csv (example)
It will create a ';' delimited csv file with a header for each table. The filename is set on the table object.
DO NOT edit this header if you are trying to use this as it is right now. 

A very basic check is done after some operations to check if the csv header is there, it basically just checks if 
the first line (header) matches the header string generated for that table and if not insert a new line with header. 

If you made changes to any header text this will mean that the line doesnt match and a new one is inserted, we now 
have two, not good. This will cause parsing issues.

Lets say the header of a datetime column is [CreateDate], and this value is moved to line two wich is the first line
read, header line removed before this operation, the string [CreateDate] will not be a valid datetime.

If this happens just open the csv file and remove the second line, the header that was moved down.

## TextDbInfo.json
The library also creates a json file over the database schema, but this file looks a little bit
wonky, will have to look more in to this, never worked with json before. This file is generated
mostly for fun as a test at the moment.

# Database Examples
Here are some examples of operation that you can do on the database.

## Table example
- A table setup in your class that inherits from TextDbSchema
```C#
public IDbTableSet PersonsTbl
{
    get
    {
        IReadOnlyList<IDbColumn> columns = new List<IDbColumn>
        {
            new DbPrimaryKeyColumn<int>("Id", 0, ColumnDataType.Int),
            new DbColumn("FirstName", 1, ColumnDataType.String),
            new DbColumn("LastName", 2, ColumnDataType.String),
            new DbColumn("EmailAddress", 3, ColumnDataType.String),
            new DbColumn("CellphoneNumber", 4, ColumnDataType.String),
            new DbParseableColumn<DateTime>("CreateDate", 5, ColumnDataType.DateTime),
            new DbRelationshipColumn("Prize", 5, ColumnDataType.MultipleRelationships, typeof(Prize), "PrizesTbl")
        };
        string dbTextFile = "PersonModels.csv";
        string tableName = "PersonsTbl";

        var tblSet = new DbTableSet<Person>(columns, dbTextFile, tableName);

        return tblSet;
    }
}
```

## Entity example
- A entity for the database has to implement either IPrimaryInt or IPrimaryString interface
- String ids is GUID's
```C#
public class Person : IPrimaryInt
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string CellphoneNumber { get; set; }
    public DateTime CreateDate { get; set; }
    public List<Prize> Prize { get; set; } = new List<Prize>();
}

public class Matchup : IPrimaryString
{
    public string Id { get; set; }
    public List<MatchupEntry> Entries { get; set; } = new List<MatchupEntry>();
    public Team Winner { get; set; }
    public int MatchupRound { get; set; }
}
```

## CRUD examples
```C#
// New up of your class that inherits from TextDbSchema, in this case DbContext
var db = new DbContext();

// Add entity, returns entity with new id
db.PersonsTbl.Add(entity);

// Read entity by id
db.PersonsTbl.Read<Person>(1);

// Update entity, returns entity
db.PersonsTbl.Update(entity);

// Delete entity, relations to this entity will be deleted as well, returns bool
db.PersonTbl.Delete(entity);

// List all
db.PersonsTbl.List<Person>();

// AddEntities, add a list<entity> to the database, returns list with new ids
db.PersonsTbl.AddEntities(entityList);
```

## Sql Examples 
- This will give you a DataSet with the columns Id, FirstName and LastName that matches the condition.
```SQL
Select [Id], [FirstName], [LastName] From [PersonsTbl] Where [FirstName] == 'Robert' && [LastName] != 'Lundgren'
```

- This will give you a DataSet with all the columns that matches the condition.
```SQL
Select * From [PersonsTbl] Where [FirstName] == 'Robert' && [LastName] != 'Lundgren'
```

- This will give you a DataSet with all the columns and all the rows in a table.
```SQL
Select * From [PersonsTbl]
```

# Bugs
Yes. Probably alot at the moment. Have not cared to much about error handling and 
user input controls yet.

I expect many issues if columns are added to or deleted from tables when database file 
for that table already exists. There is no function to fix this as it is right now.

To add columns should not be a big problem to fix as long as the columns from start doesnt
get removed. My brain is working on how to fix so that columns can change under time.
If column removed from DBContext then remove entries/column from text file.

This will mean alot of string handling and so on, so this will be something that might will
be developed over time.

# Credits
I have to give some credits to [**Tim Corey**](https://www.youtube.com/user/IAmTimCorey/featured). I got the idea to make this library watching
one of his videos on youtube where he built an application using text files as a simple database and
i wanted to try to make something like that but more generic, that is somewhat scaleable and reusable
and has a little more functions.

I took the entitys from his video to get started quickly, i reused two small extension methods he wrote
to get filepath from `string` and another one to load filepath to `List<string>` and the basic ideea then 
i just went bananas.

I have learned alot watching his stuff and i want to give thanks to him for making them for us
who really wants to learn C#.

# More info to come...
Feel free to involve yourself in this if you want :)
