using System.Collections.Generic;
using System.Data;
using Outercurve.SQLiteCreateTree.Nodes;
using Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint;

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


        internal ColumnDefNode CreateColumnDefNode()
        {
            var ret = new ColumnDefNode { ColumnName = this.ColumnName, ColumnConstraints = new List<ColumnConstraintNode>() };

            //dialect converts DbType.Int16-64 to "INT" not "INTEGER" and only INTEGER columns can be autoincremented. This fixes that.
            string correctType = this.IsIdentity ? "INTEGER" : this.DbType;

            ret.TypeNameNode = SQLiteParseVisitor.ParseString<TypeNameNode>(correctType, i => i.type_name());

            //not quite right but should work

            if (this.IsIdentity || this.IsPrimaryKey)
            {
                var primKey = new PrimaryKeyConstraintNode();
                if (this.IsIdentity)
                {
                    primKey.AutoIncrement = true;
                }
                ret.ColumnConstraints.Add(primKey);
            }

            if (this.Default != null)
            {
                ret.ColumnConstraints.Add(new DefaultConstraintNode { Value = DbUtils.ConvertToSqlValue(Default) });
            }

            if (this.IsNotNull)
            {
                ret.ColumnConstraints.Add(new NotNullConstraintNode());
            }
            else if (this.Default == null && !this.IsPrimaryKey && !this.IsUnique)
            {
                ret.ColumnConstraints.Add(new DefaultConstraintNode { Value = "NULL" });
            }

            if (this.IsUnique)
            {
                ret.ColumnConstraints.Add(new UniqueConstraintNode());
            }

            return ret;
        }

    }
}
