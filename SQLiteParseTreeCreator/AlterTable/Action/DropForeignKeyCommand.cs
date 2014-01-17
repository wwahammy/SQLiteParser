

namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public class DropForeignKeyCommand : TableCommand {
        public string Name { get; private set; }

        public DropForeignKeyCommand(string srcTable, string name)
            : base(srcTable)
        {
            Name = name;
        }

        
    }
}
