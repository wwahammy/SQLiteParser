using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint
{
    public class CollateConstraintNode : ColumnConstraintNode
    {
        public CollateConstraintNode()
        {

        }

        public CollateConstraintNode(ParserRuleContext context) : base(context)
        {

        }

        public string CollationName { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
