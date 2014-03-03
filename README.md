SQLite CREATE Statement Parser for .Net
=====================
SQLite CREATE Statement Parser for .Net is a library for parsing and manipulating SQLite CREATE TABLE and CREATE INDEX statements and simulate ALTER TABLE support for SQLite.

[![Build status](https://ci.appveyor.com/api/projects/status?id=cpv09gigti5q7694)](https://ci.appveyor.com/project/sqliteparser)

Features
--------
* Simulate ALTER TABLE by creating the necessary statements for copying data from your original table into a temporary table, recreating the original table with the ALTER TABLE modifications made and copying back the appropriate data.
* Recreate any indices on the original table after the copy.
* Creates native ALTER TABLE statements if the modifications requested can be done using native ALTER TABLE statement.
* Has a fluent API for creating ALTER TABLE statements (based upon the [Outercurve][4] [Orchard][5]'s schema manipulation API).
* Parses CREATE TABLE and CREATE INDEX statements into a parse tree useful for modification.
* Provides an ANTLR4 Parser for CREATE TABLE and CREATE INDEX statements.
* Does not connect to a SQLite database directly; you handle accessing and running your database.

How to get
----------
Just install via Outercurve NuGet:
```
Install-Package Outercurve.SQLiteParser -Prerelease
```

Requirements
--------
###To Build 
- Visual Studio 2010 SP1 (I think) or equivalent

###To Run
- .Net 4 or Mono equivalent

Why?
--------
[SQLite][1] is the go-to embedded DB for open source development. Unfortunately, it has [very limited ALTER TABLE support][2]. There are situations where a developer needs to modify column types, removing a column or change a foreign key to a previously existing SQLite table. The SQLite developers have documented a [workaround][3] however this is a specific solution, there's no general solution provided. Additionally, this workaround doesn't handle indexes.

How do I?
----------
### ...alter a table?

```c#
// get your original table named "table_to_modify"
string createTable = <the result of running "SELECT sql FROM sqlite_master WHERE tbl_name = 'table_to_modify' AND type = 'table'" on your db>

IEnumerable<string> tableIndices = <the result of running "SELECT sql FROM sqlite_master WHERE tbl_name = 'table_to_modify' AND type = 'index'" on your db>

//describe your alterations to your table with an AlterTableCommand object
var input = new AlterTableCommand("table_to_modify");

//add an index for "table_to_modify" named "an_index" on columns "id" and "name"
input.CreateIndex("an_index", "id", "name");

//drop the index for "table_to_modify" named "some_index)
input.DropIndex("some_index");

// add a new column to 'table_to_modify' called "add_column" of type TINYINT
input.AddColumn("add_column", "TINYINT");
 
 //drop the column "last" on "table_to_modify"
input.DropColumn("last");

// alter column "name" on "table_to_modify" by setting the default to 0 and it's type to INTEGER
input.AlterColumn("name",c => c.WithDefault(0).WithType("INTEGER"));

//create an AlterTableAdapter
var adapter = new AlterTableAdapter(createTable, tableIndices);

//the statements to perform the AlterTable on your DB
IEnumerable<string> output = adapter.AlterTableStatements(input);

//run each of the the statements in "output" on your database
```
### ...parse a CREATE TABLE or CREATE INDEX string?
```c#
string createTable = "Create Table TEST_TestTable (id INTEGER primary key autoincrement, name TEXT NULL, last TEXT)";

SQLiteParseTreeNode statementNodes = SQLiteParseVisitor.ParseString(createTable);
```
#### ...parse only an element of a CREATE string?
```c#
string parseOnlyColumnDef = "id INTEGER primary key autoincrement";

SQLiteParseTreeNode statementNode = SQLiteParseVisitor.ParseString(createTable, i => i.column_def() /*A statement node from the Outercurve.SQLiteParser library*/);
```

### ...build the source
From a git command prompt:
```sh
> git clone https://github.com/ericschultz/SQLiteParser.git
> cd SQLiteParser
> deploy.cmd #this builds SQLiteParser.sln, SQLiteTreeCreator.sln and runs tests.
```
Understanding the source
-----
The SQLite CREATE Statement Parser for .Net consists of two libraries. First, Outercurve.SQLiteParser (individually buildable from SQLiteParser.sln) consists of the basic parsing of a SQLite CREATE statement. The grammar is purposely written to be VERY close to the original grammar from the SQLite project and available in SQLiteParser\SQLiteParserSimple.g4. That said, the tree is not intuitive to use.

The second library, Outercurve.SQLiteTreeCreator (individually buildable from SQLiteTreeCreator.sln AFTER SQLiteParser.sln has been built) has a few pieces of functionality. It can take in a CREATE statement (or the original parse tree from SQLiteParser) and creates a cleaner, more intuitive tree for manipulation. Additionally, it has a TreeStringOutputVisitor class for converting this intuitive tree back into a CREATE statement. This allows someone to manipulate a CREATE statement via an object model and then get a new, corresponding statement. On top of this, Outercurve.SQLiteTreeCreator has a Fluent API for creating ALTER TABLE statements in SQLiteParseTreeCreator\AlterTable. This Fluent API allows you to create ALTER TABLE statements via code and then create the necessary statements to simulate proper ALTER TABLE support in SQLite.

##Limitations
- The alter table simulator will likely fail to create usable statements if your foreign keys are not set to "DEFERRABLE INITIALLY DEFERRED." I think there's a way to get around this by turning off the SQLite foreign key support, modifying the tables, turn foreign key support back on and then raise a SQLite error if the foreign keys aren't valid anymore. Just didn't have time to work it out.

## FAQ:
**Q: Why not connect directly to a SQLite DB and run the statements?**    
A: There are lots of different ways to connect to a DB. The current design allows you flexibility to use any DB layer.     
**Q: Why are the two libraries in different solutions?**    
A: The ANTLR4 MSBuild tasks create intermediate C# files for the grammar classes in Outercurve.SQLiteParser. VS Intellisense wasn't picking them up which meant we'd lose all Intellisense support while working on Outercurve.SQLParseTreeCreator. There may be a work around but I never found it.    
**Q: Why no Fluent API for CREATE TABLE statements?**    
A: Honestly, I just never had time to work it through. That said, I don't think it would be very difficult to do.    

  [1]: http://sqlite.org
  [2]: http://www.sqlite.org/lang_altertable.html
  [3]: http://www.sqlite.org/faq.html#q11
  [4]: http://outercurve.org
  [5]: http://orchardproject.net
