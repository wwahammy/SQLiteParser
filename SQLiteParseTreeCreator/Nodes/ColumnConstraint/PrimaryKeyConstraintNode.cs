using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint
{
    public class PrimaryKeyConstraintNode : ColumnConstraintNode
    {
        public PrimaryKeyConstraintNode()
        {

        }

        public PrimaryKeyConstraintNode(ParserRuleContext context) : base(context)
        {

        }

        public SortOrder? Order { get; set; }

        public ConflictClauseNode ConflictClause { get; set; }

        public bool AutoIncrement { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
