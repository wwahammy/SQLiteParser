namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public class DropColumnCommand : ColumnCommand {

        public DropColumnCommand(string tableName, string columnName)
            : base(tableName, columnName) {
        }
    }
}
