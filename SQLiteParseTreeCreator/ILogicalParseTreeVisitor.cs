using Outercurve.SQLiteCreateTree.Nodes;
using Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint;
using Outercurve.SQLiteCreateTree.Nodes.TableConstraint;

namespace Outercurve.SQLiteCreateTree
{
    public interface ILogicalParseTreeVisitor<out T>
    {
        T Visit(TypeNameNode typeNameNode);
        T Visit(CreateTableNode createTableNode);
        T Visit(ColumnDefNode columnDefNode);
        T Visit(NotNullConstraintNode notNullColumnConstraintNode);
        T Visit(PrimaryKeyConstraintNode primaryKeyColumnConstraintNode);
        T Visit(ConflictClauseNode conflictClauseNode);
        T Visit(UniqueConstraintNode uniqueConstraintNode);
        T Visit(CheckConstraintNode checkConstraintNode);
        T Visit(DefaultConstraintNode defaultConstraintNode);
        T Visit(CollateConstraintNode collateConstraintNode);
        T Visit(ForeignDeferrableNode foreignDeferrableNode);
        T Visit(ForeignOnUpdateNode foreignOnUpdateNode);
        T Visit(ForeignOnDeleteNode foreignOnDeleteNode);
        T Visit(ForeignMatchNode foreignMatchNode);
        T Visit(IndexClauseNode indexClause);
        T Visit(IndexedColumnNode indexedColumnNode);
        T Visit(TableConstraintCheckNode tableConstraintCheckNode);
        T Visit(TableConstraintForeignKeyNode tableConstraintForeignKeyNode);
        T Visit(ForeignKeyClauseNode foreignKeyClauseNode);
    }
}
