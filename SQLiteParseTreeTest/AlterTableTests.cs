using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExpectedObjects;
using Outercurve.SQLiteCreateTree;
using Outercurve.SQLiteCreateTree.AlterTable;
using Outercurve.SQLiteCreateTree.AlterTable.Action;
using Outercurve.SQLiteCreateTree.Nodes;
using Xunit;

namespace SQLiteParseTreeTest
{
    public class AlterTableTests
    {
        [Fact]
        public void FirstAlterTableTest()
        {
            var originalTable =
                "Create Table TEST_TestTable (id INTEGER primary key autoincrement, name TEXT(40) NULL, last TEXT(256))";
            var originalIndices = new[]
            {
                "CREATE INDEX some_index ON TEST_TestTable (id)",
                "CREATE INDEX name_index ON TEST_TestTable (name)",
                "CREATE INDEX this_should_be_gone ON TEST_TestTable (name, last)",
            };

            

            var input = new AlterTableCommand("TEST_TestTable");
            input.CreateIndex("funny_index", "id", "name");
            input.DropIndex("some_index");
            input.AddColumn("add_column", "TINYINT", command => command.WithType("TINYINT"));
            input.DropColumn("last");
            //no, this doesn't make sense; no, I don't care
            input.AlterColumn("name",c => c.WithDefault(0).WithType("INTEGER"));


            var expectedFinal = new[]
            {
                //this line expects the guid to be added
                "CREATE TEMPORARY TABLE TEST_TestTable_ AS SELECT * FROM TEST_TestTable;",
                "DROP TABLE TEST_TestTable;",
                "CREATE TABLE TEST_TestTable (id INTEGER primary key autoincrement, name INTEGER default 0, add_column TINYINT default NULL);",
                //this line expects the guid to be added
                "INSERT INTO TEST_TestTable (id, name) SELECT id, name FROM TEST_TestTable_;",
                "CREATE INDEX name_index ON TEST_TestTable (name);",
                "CREATE INDEX funny_index ON TEST_TestTable (id, name);",
                //this line expects the guid to be added
                "DROP TABLE TEST_TestTable_;",
            }.Select(LowerAndWhitespaceFreeString).ToArray();

            var adapter = new AlterTableAdapter(SQLiteParseVisitor.ParseString<CreateTableNode>(originalTable),
                originalIndices.Select(SQLiteParseVisitor.ParseString<CreateIndexNode>));

            var output = adapter.AlterTableStatements(input).Select(LowerAndWhitespaceFreeString).ToArray();

            Assert.Equal(expectedFinal.Length, output.Length);

            string tempTableName = "test_testtable_";
            for (int i = 0; i < expectedFinal.Length; i++)
            {

                var testValue = expectedFinal[i];

                if (i == 0)
                {
                    //get the guid from the output
                    tempTableName += Regex.Match(output[i], "[0-9a-fA-F]{32}").Value;

                }
                testValue = testValue.Replace("test_testtable_", tempTableName);
                Assert.Equal(testValue, output[i]);
            }
        }

        
        [Fact]
    
        public void HandleAlterTables()
        {
            var originalTable =
                "Create Table TEST_TestTable (id INTEGER primary key autoincrement, name TEXT(40) NULL, last TEXT(256), "
            + "CONSTRAINT TEST_FK FOREIGN KEY (id) REFERENCES FAKE_TABLE (id) DEFERRABLE INITIALLY DEFERRED,"
            + "CONSTRAINT KEEP_FK FOREIGN KEY (temp, id) REFERENCES FAKE_TABLE (temp, id) DEFERRABLE INITIALLY DEFERRED);";
            var originalIndices = new[]
            {
                "CREATE INDEX some_index ON TEST_TestTable (id)",
                "CREATE INDEX name_index ON TEST_TestTable (name)",
                "CREATE INDEX this_should_be_gone ON TEST_TestTable (name, last)",
            };

            var input = new AlterTableCommand("TEST_TestTable");
            input.CreateIndex("funny_index", "id", "name");
            input.DropIndex("some_index");
            input.AddColumn("add_column", "TINYINT", command => command.WithType("TINYINT"));
            input.DropColumn("last");
            //no, this doesn't make sense; no, I don't care
            input.AlterColumn("name", c => c.WithDefault(0).WithType("INTEGER"));
            input.DropForeignKey("TEST_FK");
            input.CreateForeignKey("TEST_FK2", new []{"id", "last"}, "SOME_OTHERTABLE", new []{"something", "else"});

#if false
            var expectedObject = new
            {
                Name = "TEST_TestTable",
                TableCommands = new List<object>
                {
                    //funny index
                    new 
                    {
                       Name= "funny_index",
                       TableName = "TEST_TestTable",
                       ColumnNames = new object[] { "id", "name"}
                    },
                    //dropindex

                    new
                    {
                        name= "some_index",
                        TableName ="TEST_TestTable"
                    },

                    new
                    {
                        TableName = "TEST_TestTable",
                        ColumnName = "add_column",
                        DbType = "TINYINT",
                    },
                    new
                    {
                        TableName = "TEST_TestTable",
                        ColumnName = "last"
                    },

                     new
                    {
                        TableName = "TEST_TestTable",
                        ColumnName = "name",
                        DbType = "Integer",
                        Default = "0"
                    },
                    new
                    {
                        Name = "TEST_FK",
                        TableName = "TEST_TestTable"
                    },

                    new
                    {
                        TableName = "TEST_TestTable",
                        Name="TEST_FK2",
                        DestColumns =  new [] {"id", "last"},
                        DestTable = "SOME_OTHERTABLE",
                        SrcColumns = new []{"id", "last"}
                    }


                }
            }.ToExpectedObject();

            expectedObject.ShouldMatch(input);
#endif

            var expectedFinal = new[]
            {
                //this line expects the guid to be added
                "CREATE TEMPORARY TABLE TEST_TestTable_ AS SELECT * FROM TEST_TestTable;",
                "DROP TABLE TEST_TestTable;",
                "CREATE TABLE TEST_TestTable (id INTEGER primary key autoincrement, name INTEGER default 0, add_column TINYINT default NULL " +
                    "CONSTRAINT KEEP_FK  FOREIGN KEY (temp, id) REFERENCES FAKE_TABLE (temp, id) DEFERRABLE INIITALLY DEFERRED, "+
                    "CONSTRAINT TEST_FK2  FOREIGN KEY (id, last) REFERENCES SOME_OTHERTABLE (something, else) DEFERRABLE INIITALLY DEFERRED);",
                //this line expects the guid to be added
                "INSERT INTO TEST_TestTable (id, name) SELECT id, name FROM TEST_TestTable_;",
                "CREATE INDEX name_index ON TEST_TestTable (name);",
                "CREATE INDEX funny_index ON TEST_TestTable (id, name);",
                //this line expects the guid to be added
                "DROP TABLE TEST_TestTable_;",
            }.Select(LowerAndWhitespaceFreeString).ToArray();


            var adapter = new AlterTableAdapter(SQLiteParseVisitor.ParseString<CreateTableNode>(originalTable),
                originalIndices.Select(SQLiteParseVisitor.ParseString<CreateIndexNode>));

            var output = adapter.AlterTableStatements(input).Select(LowerAndWhitespaceFreeString).ToArray();

            Assert.Equal(expectedFinal.Length, output.Length);

            string tempTableName = "test_testtable_";
            for (int i = 0; i < expectedFinal.Length; i++)
            {

                var testValue = expectedFinal[i];

                if (i == 0)
                {
                    //get the guid from the output
                    tempTableName += Regex.Match(output[i], "[0-9a-fA-F]{32}").Value;

                }
                testValue = testValue.Replace("test_testtable_", tempTableName);
                Assert.Equal(testValue, output[i]);
            }
        }

        public void DropMissingColumnShouldFail()
        {
            //var origin
        }

        public void DropMissingIndexShouldFail()
        {

        }

        private string LowerAndWhitespaceFreeString(string i)
        {
            return Regex.Replace(i, @"\s+", "").ToLowerInvariant();
        }

    }
}
