// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Outercurve.SQLiteParser;
using Xunit;

namespace BasicParserTests
{
    public class VeryBasicParserTest
    {
        [Fact]
        public void CanWeParseWithoutErrorsTest()
        {
            var queryString = "CREATE TABLE c15 (x, y, FOREIGN KEY(x,y) REFERENCES p5(b,c))";
            var inputStream = new AntlrInputStream(queryString);
            var sqliteLexer = new SQLiteParserSimpleLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(sqliteLexer);
            var sqliteParser = new SQLiteParserSimpleParser(commonTokenStream);
            
            var s = sqliteParser.sql_stmt();
            Assert.Equal(0, sqliteParser.NumberOfSyntaxErrors);
        }
    }
}
