// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public class AddIndexCommand : TableCommand {
        public string IndexName { get; set; }

        public AddIndexCommand(string tableName, string indexName, params string[] columnNames)
            : base(tableName) {
            ColumnNames = columnNames;
            IndexName = indexName;
        }

        public string[] ColumnNames { get; private set; }
    }
}
