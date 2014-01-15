using System.Data;

namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public class CreateColumnCommand : ColumnCommand {
        public CreateColumnCommand(string tableName, string name) : base(tableName, name) {
            IsNotNull = false;
            IsUnique = false;
        }

        public bool IsUnique { get; protected set; }

        public bool IsNotNull { get; protected set; }

        public bool IsPrimaryKey { get; protected set; }

        public bool IsIdentity { get; protected set; }

        public CreateColumnCommand PrimaryKey() {
            IsPrimaryKey = true;
            IsUnique = false;
            return this;
        }

        public CreateColumnCommand Identity() {
            IsIdentity = true;
            IsUnique = false;
            return this;
        }


        public CreateColumnCommand NotNull() {
            IsNotNull = true;
            return this;
        }

        public CreateColumnCommand Nullable() {
            IsNotNull = false;
            return this;
        }

        public CreateColumnCommand Unique() {
            IsUnique = true;
            IsPrimaryKey = false;
            IsIdentity = false;
            return this;
        }

        public CreateColumnCommand NotUnique() {
            IsUnique = false;
            return this;
        }

       
        public new CreateColumnCommand WithType(string dbType) {
            base.WithType(dbType);
            return this;
        }

    }
}
