using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outercurve.SQLiteCreateTree.AlterTable
{
    internal static class IndexWriter
    {
        internal static string AddIndex(string indexName, string tableName, IEnumerable<string> columnNames)
        {
            return String.Format("CREATE INDEX {0} ON {1} ({2});", indexName, tableName, string.Join(",", columnNames));
        }

        internal static string DropIndex(string indexName)
        {
            return String.Format("DROP INDEX {0};", indexName);
        }
        
    }
}
