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

# Examples

<b>var db = new DbContext();</b>

- Add entity, returns entity with new id =><br/>
<b>db.PersonsTbl.Add(entity);</b><br/>
<br/>
- Read entity by id =><br/>
<b>db.PersonsTbl.Read<Person>(1);</b><br/>
<br/>
- Update entity, returns entity => <br/>
<b>db.PersonsTbl.Update(entity);</b><br/>
<br/>
- Delete entity, relations to this entity will be deleted as well => <br/>
<b>db.PersonTbl.Delete(entity);</b><br/>
<br/>
- List all<br><br/>
<b>db.PersonsTbl.List<Person>();</b><br/>
<br/>
- AddEntities, add a list<entity> to the database<br/>
<b>db.PersonsTbl.AddEntities(entityList);</b><br/>
<br/>
# More info to come...
Feel free to involve yourself in this if you want :)
