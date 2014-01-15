using System.Data;

namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public class ColumnCommand : TableCommand {
        public string ColumnName { get; set; }

        public ColumnCommand(string tableName, string name)
            : base(tableName) {
            ColumnName = name;
            DbType = null;
            Default = null;
        }

        public string DbType { get; private set; }

        public object Default { get; private set; }


        public ColumnCommand WithType(string dbType) {
            DbType = dbType;
            return this;
        }

        public ColumnCommand WithDefault(object @default) {
            Default = @default;
            return this;
        }
    }
}
