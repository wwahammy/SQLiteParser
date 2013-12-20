using Antlr4.Runtime;

namespace SQLiteParseTreeCreator.Nodes.ColumnConstraint
{
    public class UniqueConstraintNode : ColumnConstraintNode
    {
        public UniqueConstraintNode()
        {

        }

        public UniqueConstraintNode(ParserRuleContext context) : base(context)
        {

        }

        public ConflictClauseNode ConflictClause { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
