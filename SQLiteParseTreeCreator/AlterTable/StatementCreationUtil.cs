// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outercurve.SQLiteCreateTree.AlterTable
{
    internal static class StatementUtil
    {
        internal static string AddColumn(string tableName, string columnDefString)
        {
            return String.Format("ALTER TABLE {0} ADD COLUMN {1}", tableName, columnDefString);
        }

        internal static string AddIndex(string indexName, string tableName, IEnumerable<string> columnNames)
        {
            return String.Format("CREATE INDEX {0} ON {1} ({2});", indexName, tableName, string.Join(",", columnNames));
        }

        internal static string DropIndex(string indexName)
        {
            return String.Format("DROP INDEX {0};", indexName);
        }

        internal static string CreateTempTable(string tempTable, string originalTable)
        {
            return String.Format("CREATE TEMPORARY TABLE {0} AS SELECT * FROM {1};", tempTable, originalTable);
        }

        internal static string DropTable(string tableName)
        {
            return String.Format("DROP TABLE {0};", tableName);
        }


        internal static string InsertInto(string toTable, IEnumerable<string> columnNames, string fromTable)
        {
            return String.Format("INSERT INTO {0} ({1}) SELECT {1} FROM {2};", toTable, string.Join(",", columnNames), fromTable);
        }

    }
}
