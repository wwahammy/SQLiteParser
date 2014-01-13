using System.Linq;
using System.Text;
using Outercurve.SQLiteCreateTree.Nodes;
using Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint;
using Outercurve.SQLiteCreateTree.Nodes.TableConstraint;

namespace Outercurve.SQLiteCreateTree
{
    public class TreeStringOutputVisitor : ILogicalParseTreeVisitor<StringBuilder>
    {
        public TreeStringOutputVisitor()
        {
            
        }

        public StringBuilder Visit(TypeNameNode typeNameNode)
        {


            var sb = new StringBuilder();
            sb.Append(typeNameNode.TypeName);
            sb.Append(" ");
            if (typeNameNode.SignedNumbers != null && typeNameNode.SignedNumbers.Any())
            {
                sb.Append("(");
                // I'm Lazy
                sb.Append(string.Join(", ", typeNameNode.SignedNumbers));
                sb.Append(") ");
            }


            return sb;
        }
        public StringBuilder Visit(CreateTableNode createTableNode)
        {

            var sb = new StringBuilder();
            sb = sb.Append("CREATE ");
            if (createTableNode.Temp)
            {
                sb.Append("TEMPORARY ");
            }

            if (createTableNode.IfNotExists)
            {
                sb.Append("IF NOT EXISTS ");
            }

            sb.Append("TABLE ");

            sb.Append(createTableNode.TableName);

            if (createTableNode.HasSelectStmt)
            {
                sb.Append("AS ");
                sb.Append(createTableNode.SelectStmt);
            }
            else
            {
                sb.Append("(");
                //column definitions

                sb.Append(string.Join(", ", createTableNode.ColumnDefinitions.Select(c => c.Accept(this).ToString())));


                if (createTableNode.TableConstraints != null && createTableNode.TableConstraints.Any())
                {
                    sb.Append(", ");
                    sb.Append(string.Join(", ", createTableNode.TableConstraints.Select(c => c.Accept(this).ToString())));
                }

                sb.Append(") ");

                if (createTableNode.WithoutRowId)
                {
                    sb.Append("WITHOUT ROWID ");
                }
            }
            sb.Append(";");

            return sb;
        }

        public StringBuilder Visit(ColumnDefNode columnDefNode)
        {
            var sb = new StringBuilder();
            sb.Append(columnDefNode.ColumnName);
            
            if (columnDefNode.TypeNameNode != null)
            {
                sb.Append(" ");
                sb.Append(columnDefNode.TypeNameNode.Accept(this));

            }

            if (columnDefNode.ColumnConstraints != null && columnDefNode.ColumnConstraints.Any())
            {
                foreach (var constraint in columnDefNode.ColumnConstraints)
                {
                    sb.Append(" ");
                    sb.Append(constraint.Accept(this));
                }
                
            }

            return sb;
           
        }

        public StringBuilder Visit(NotNullConstraintNode notNullColumnConstraintNode)
        {
            var sb = new StringBuilder();
            sb.Append("NOT NULL ");
            if (notNullColumnConstraintNode.ConflictClause != null)
            {
                sb.Append(notNullColumnConstraintNode.ConflictClause.Accept(this));
            }

            return sb;
        }

        public StringBuilder Visit(PrimaryKeyConstraintNode primaryKeyColumnConstraintNode)
        {
            var sb = new StringBuilder();
            sb.Append("PRIMARY KEY ");
            if (primaryKeyColumnConstraintNode.Order != null)
            {
                sb.Append(primaryKeyColumnConstraintNode.Order.Value.ToString());
                sb.Append(" ");
            }
            if (primaryKeyColumnConstraintNode.ConflictClause != null)
            {
                sb.Append(primaryKeyColumnConstraintNode.ConflictClause.Accept(this));
            }
            if (primaryKeyColumnConstraintNode.AutoIncrement)
            {
                sb.Append(" AUTOINCREMENT");
            }

            return sb;
        }

        public StringBuilder Visit(UniqueConstraintNode uniqueConstraintNode)
        {
            var sb = new StringBuilder();
            sb.Append("UNIQUE ");
            sb.Append(uniqueConstraintNode.ConflictClause.Accept(this));

            return sb;
        }

        public StringBuilder Visit(CheckConstraintNode checkConstraintNode)
        {
            var sb = new StringBuilder();
            sb.Append("CHECK (");
            sb.Append(checkConstraintNode.Expr);
            sb.Append(")");

            return sb;
        }

        public StringBuilder Visit(DefaultConstraintNode defaultConstraintNode)
        {
            var sb = new StringBuilder();
            sb.Append("DEFAULT ");
            sb.Append(defaultConstraintNode.Value);
            return sb;
        }

        public StringBuilder Visit(CollateConstraintNode collateConstraintNode)
        {
            var sb = new StringBuilder("DEFAULT ");
            sb.Append(collateConstraintNode.CollationName);
            return sb;
        }

        public StringBuilder Visit(ForeignKeyClauseNode foreignKeyClauseNode)
        {
            var sb = new StringBuilder("REFERENCES ");
            sb.Append(foreignKeyClauseNode.TableName);
            sb.Append(" ");

            if (foreignKeyClauseNode.FieldList != null)
            {
                foreach (var field in foreignKeyClauseNode.FieldList)
                {
                    sb.Append(" ");
                    sb.Append(field);
                }
            }

            foreach (var delete in foreignKeyClauseNode.ForeignOnDelete)
            {
                sb.Append(" ");
                sb.Append(delete.Accept(this));
            }

            foreach (var update in foreignKeyClauseNode.ForeignOnUpdate)
            {
                sb.Append(" ");
                sb.Append(update.Accept(this));
            }

            foreach (var match in foreignKeyClauseNode.ForeignMatch)
            {
                sb.Append(" ");
                sb.Append(match.Accept(this));
            }

            if (foreignKeyClauseNode.ForeignDeferrable != null)
            {
                sb.Append(" ");
                sb.Append(foreignKeyClauseNode.ForeignDeferrable.Accept(this));
            }

            return sb;
        }

        public StringBuilder Visit(CreateIndexNode createIndexNode)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE ");
            if (createIndexNode.IsUnique)
            {
                sb.Append("UNIQUE ");
            }
            sb.Append("INDEX ");
            if (createIndexNode.DatabaseName != null)
            {
                sb.Append(createIndexNode.DatabaseName).Append(".");
            }

            sb.Append(createIndexNode.IndexName);
            sb.Append(" ON ");
            sb.Append(createIndexNode.TableName);
            sb.Append(" (");

            sb.Append(string.Join(", ", createIndexNode.IndexedColumnNodes.Select(i => i.Accept(this))));
            sb.Append(")");
            if (createIndexNode.WhereExpr != null)
            {
                sb.Append(" WHERE ");
                sb.Append(createIndexNode.WhereExpr);
            }

            sb.Append(";");

            return sb;
        }


        public StringBuilder Visit(ConflictClauseNode conflictClauseNode)
        {
            var sb = new StringBuilder();

            sb.Append("ON CONFLICT ");
            switch (conflictClauseNode.Choice)
            {
                case ConflictChoice.Abort:
                    sb.Append("ABORT");
                    break;
                case ConflictChoice.Fail:
                    sb.Append("FAIL");
                    break;
                case ConflictChoice.Ignore:
                    sb.Append("IGNORE");
                    break;
                case ConflictChoice.Replace:
                    sb.Append("REPLACE");
                    break;
                case ConflictChoice.Rollback:
                    sb.Append("ROLLBACK");
                    break;
            }

            return sb;
        }

        public StringBuilder Visit(ForeignDeferrableNode foreignDeferrableNode)
        {
            var sb = new StringBuilder();
            if (!foreignDeferrableNode.IsDeferrable)
            {
                sb.Append("NOT ");
            }

            sb.Append("DEFERRABLE ");

            if (foreignDeferrableNode.InitiallyImmediate != null)
            {
                if (foreignDeferrableNode.InitiallyImmediate.Value)
                {
                    sb.Append("INITALLY IMMEDIATE");
                }
                else
                {
                    sb.Append("INITIALLY DEFERRED");
                }
            }

            return sb;
        }

        public StringBuilder Visit(ForeignOnUpdateNode foreignOnUpdateNode)
        {
            var sb = new StringBuilder();
            sb.Append("ON UPDATE ");
            AppendDeleteOrUpdateAction(sb, foreignOnUpdateNode.Action);

            return sb;
        }

        public StringBuilder Visit(ForeignOnDeleteNode foreignOnDeleteNode)
        {
            var sb = new StringBuilder();
            sb.Append("ON DELETE ");
            AppendDeleteOrUpdateAction(sb, foreignOnDeleteNode.Action);

            return sb;
        }

        public StringBuilder Visit(ForeignMatchNode foreignMatchNode)
        {
            var sb = new StringBuilder();
            sb.Append("MATCH ");
            sb.Append(foreignMatchNode.Id);
            return sb;
        }

        public StringBuilder Visit(IndexClauseNode indexClause)
        {
            var sb = new StringBuilder();

            AppendConstraint(indexClause, sb);
            switch (indexClause.IndexUniqueness)
            {
                case PrimaryKeyOrUnique.PrimaryKey:
                    sb.Append("PRIMARY KEY");
                    break;
                case PrimaryKeyOrUnique.Unique:
                    sb.Append("UNIQUE");
                    break;
            }

            
            if (indexClause.IndexColumns != null)
            {
                sb.Append("(");
                sb.Append(string.Join(", ", indexClause.IndexColumns.Select(i => i.Accept(this))));
                sb.Append(") ");

            }
            if (indexClause.ConflictClauseNode != null)
            {
                sb.Append(" ");
                sb.Append(indexClause.ConflictClauseNode.Accept(this));
            }

            return sb;
        }

        public StringBuilder Visit(IndexedColumnNode indexedColumnNode)
        {
            var sb = new StringBuilder();
            sb.Append(indexedColumnNode.Id);
            
            if (indexedColumnNode.CollationId != null)
            {
                sb.Append(" COLLATE ");
                sb.Append(indexedColumnNode.CollationId);
            }

            if (indexedColumnNode.Order != null)
            {
                if (indexedColumnNode.Order.Value == SortOrder.Asc)
                {
                    sb.Append(" ASC");
                }
                else if (indexedColumnNode.Order.Value == SortOrder.Desc)
                {
                    sb.Append(" DESC");
                }
            }

            return sb;
        }

        public StringBuilder Visit(TableConstraintCheckNode tableConstraintCheckNode)
        {
            var sb = new StringBuilder();
            AppendConstraint(tableConstraintCheckNode, sb);
            sb.Append("CHECK (");
            sb.Append(tableConstraintCheckNode.Expr);
            sb.Append(")");
            return sb;
        }

        public StringBuilder Visit(TableConstraintForeignKeyNode tableConstraintForeignKeyNode)
        {
            var sb = new StringBuilder();
            AppendConstraint(tableConstraintForeignKeyNode, sb);

            sb.Append("FOREIGN KEY (");
            sb.Append(string.Join(", ", tableConstraintForeignKeyNode.FieldNames));
            sb.Append(") ");

            if (tableConstraintForeignKeyNode.ForeignKeyClauseNode != null)
            {
                sb.Append(tableConstraintForeignKeyNode.ForeignKeyClauseNode.Accept(this));
            }

            return sb;
        }

        private void AppendDeleteOrUpdateAction(StringBuilder sb, ForeignDeleteOrUpdateAction action)
        {
            switch (action)
            {
                case ForeignDeleteOrUpdateAction.Cascade:
                    sb.Append("CASCADE");
                    break;
                case ForeignDeleteOrUpdateAction.NoAction:
                    sb.Append("NO ACTION");
                    break;
                case ForeignDeleteOrUpdateAction.Restrict:
                    sb.Append("RESTRICT");
                    break;
                case ForeignDeleteOrUpdateAction.SetDefault:
                    sb.Append("SET DEFAULT");
                    break;
                case ForeignDeleteOrUpdateAction.SetNull:
                    sb.Append("SET NULL");
                    break;
            }

            
        }

        private void AppendConstraint(TableConstraintNode node, StringBuilder sb)
        {
            if (node.ConstraintName != null)
            {
                sb.Append("CONSTRAINT ");
                sb.Append(node.ConstraintName);
                sb.Append(" ");
            }
        }
    }
}
