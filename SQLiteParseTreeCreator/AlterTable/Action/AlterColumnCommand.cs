// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System.Data;

namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public class AlterColumnCommand : ColumnCommand {
        public AlterColumnCommand(string tableName, string columnName)
            : base(tableName, columnName) {
        }

        public new AlterColumnCommand WithType(string dbType) {
            base.WithType(dbType);
            return this;
        }
    }
}
