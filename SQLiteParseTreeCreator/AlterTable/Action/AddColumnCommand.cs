namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public class AddColumnCommand : CreateColumnCommand {
        public AddColumnCommand(string tableName, string name) : base(tableName, name) {
        }
    }
}
