// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ExpectedObjects;
using Outercurve.SQLiteCreateTree;
using Outercurve.SQLiteCreateTree.AlterTable;
using Outercurve.SQLiteCreateTree.AlterTable.Action;
using Outercurve.SQLiteCreateTree.Nodes;
using Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint;
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
            };

            var adapter = new AlterTableAdapter(originalTable, originalIndices);

            var output = adapter.AlterTableStatements(input).Select(LowerAndWhitespaceFreeString).ToArray();

            VerifyYourStatementsAreValid(expectedFinal, output);
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
            input.AddColumn("add_column", "TINYINT");
            input.DropColumn("last");
            //no, this doesn't make sense; no, I don't care
            input.AlterColumn("name", c => c.WithDefault(0).WithType("INTEGER"));
            input.DropForeignKey("TEST_FK");
            input.CreateForeignKey("TEST_FK2", new []{"id", "last"}, "SOME_OTHERTABLE", new []{"something", "else"});


            var expectedFinal = new[]
            {
                //this line expects the guid to be added
                "CREATE TEMPORARY TABLE TEST_TestTable_ AS SELECT * FROM TEST_TestTable;",
                "DROP TABLE TEST_TestTable;",
                "CREATE TABLE TEST_TestTable (id INTEGER primary key autoincrement, name INTEGER default 0, add_column TINYINT default NULL, " +
                    "CONSTRAINT KEEP_FK  FOREIGN KEY (temp, id) REFERENCES FAKE_TABLE (temp, id) DEFERRABLE INITIALLY DEFERRED, "+
                    "CONSTRAINT TEST_FK2  FOREIGN KEY (id, last) REFERENCES SOME_OTHERTABLE (something, else) DEFERRABLE INITIALLY DEFERRED);",
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

            VerifyYourStatementsAreValid(expectedFinal, output);
        }

        [Fact]
        public void DropMissingColumnShouldFail()
        {
            //var origin
            var originalTable =
                "Create Table TEST_TestTable (id INTEGER primary key autoincrement, name TEXT(40) NULL, last TEXT(256));";
            var originalIndices = new string[0];

            var input = new AlterTableCommand("TEST_TestTable");
            input.DropColumn("fake_column");

            var adapter = new AlterTableAdapter(SQLiteParseVisitor.ParseString<CreateTableNode>(originalTable),
                originalIndices.Select(SQLiteParseVisitor.ParseString<CreateIndexNode>));

            Assert.Throws<InvalidColumnException<DropColumnCommand>>(() => adapter.AlterTableStatements(input).ToArray());


        }

        [Fact]
        public void AlterMissingColumnShouldFail()
        {
       
            //var origin
            var originalTable =
                "Create Table TEST_TestTable (id INTEGER primary key autoincrement, name TEXT(40) NULL, last TEXT(256));";
            var originalIndices = new string[0];

            var input = new AlterTableCommand("TEST_TestTable");
            input.AlterColumn("fake_column", i => i.WithType("fake"));

            var adapter = new AlterTableAdapter(SQLiteParseVisitor.ParseString<CreateTableNode>(originalTable),
                originalIndices.Select(SQLiteParseVisitor.ParseString<CreateIndexNode>));

            Assert.Throws<InvalidColumnException<AlterColumnCommand>>(() => adapter.AlterTableStatements(input).ToArray());


       
        }


        [Fact]
        public void AlterTableAdapterIgnoresEmptyOrNullIndices()
        {
            var originalTable =
                "Create Table TEST_TestTable (id INTEGER primary key autoincrement, name TEXT(40) NULL, last TEXT(256));";
            var originalIndices = new string[0];

            var adapter = new AlterTableAdapter(SQLiteParseVisitor.ParseString<CreateTableNode>(originalTable),
                originalIndices.Select(SQLiteParseVisitor.ParseString<CreateIndexNode>));

            var input = new AlterTableCommand("TEST_TestTable");
            input.DropColumn("name");
            input.DropColumn("last");
            input.CreateIndex("fun", "id");

            var result = adapter.AlterTableStatements(input);


            var expectedFinal = new[]
            {
                //this line expects the guid to be added
                "CREATE TEMPORARY TABLE TEST_TestTable_ AS SELECT * FROM TEST_TestTable;",
                "DROP TABLE TEST_TestTable;",
                "CREATE TABLE TEST_TestTable (id INTEGER primary key autoincrement);",
                //this line expects the guid to be added
                "INSERT INTO TEST_TestTable (id) SELECT id FROM TEST_TestTable_;",
                "CREATE INDEX fun ON TEST_TestTable (id);",
                //this line expects the guid to be added
                "DROP TABLE TEST_TestTable_;",
            };

            VerifyYourStatementsAreValid(expectedFinal, result);
        }

        [Fact]
        public void DropMissingIndexShouldFail()
        {
            var originalTable =
                "Create Table TEST_TestTable (id INTEGER primary key autoincrement, name TEXT(40) NULL, last TEXT(256));";
            var originalIndices = new string[0];

            var input = new AlterTableCommand("TEST_TestTable");
            input.DropIndex("fake_index");
            input.DropColumn("name");

            var adapter = new AlterTableAdapter(SQLiteParseVisitor.ParseString<CreateTableNode>(originalTable),
                originalIndices.Select(SQLiteParseVisitor.ParseString<CreateIndexNode>));

            Assert.Throws<InvalidIndexException>(() => adapter.AlterTableStatements(input).ToArray());

        }

        [Fact]
        public void ParseStringWithParseErrorsFails()
        {
            // missing one right parens at the end
            var originalTable =
                "Create Table TEST_TestTable (id INTEGER primary key autoincrement, name TEXT(40) NULL, last TEXT(256)";

            Assert.Throws<ParseException>(() => SQLiteParseVisitor.ParseString<CreateTableNode>(originalTable));
        }

        [Fact]
        public void ParsePartOfAString()
        {
            string parseOnlyAnArgument = "id INTEGER primary key autoincrement";

            SQLiteParseTreeNode statementNodes = SQLiteParseVisitor.ParseString(parseOnlyAnArgument, i => i.column_def());

            var expected = new ColumnDefNode()
            {
                ColumnName = "id",
                ColumnConstraints = new[]
                {
                    new PrimaryKeyConstraintNode {AutoIncrement = true}
                },
                TypeNameNode = new TypeNameNode() {TypeName = "INTEGER"}
            }.ToExpectedObject().AddTreeNode();

            expected.ShouldMatch(statementNodes);

        }

        private string LowerAndWhitespaceFreeString(string i)
        {
            return Regex.Replace(i, @"\s+", "").ToLowerInvariant();
        }


        private void VerifyYourStatementsAreValid(IEnumerable<string> expected, IEnumerable<string> actual)
        {
            var expectedArray = expected.Select(LowerAndWhitespaceFreeString).ToArray();
            var actualArray = actual.Select(LowerAndWhitespaceFreeString).ToArray();

            Assert.Equal(expectedArray.Length, actualArray.Length);

            string tempTableName = "test_testtable_";
            for (int i = 0; i < expectedArray.Length; i++)
            {

                var testValue = expectedArray[i];

                if (i == 0)
                {
                    //get the guid from the output
                    tempTableName += Regex.Match(actualArray[i], "[0-9a-fA-F]{32}").Value;

                }
                testValue = testValue.Replace("test_testtable_", tempTableName);
                Assert.Equal(testValue, actualArray[i]);
            }
        }

    }
}
