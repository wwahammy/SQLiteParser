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
