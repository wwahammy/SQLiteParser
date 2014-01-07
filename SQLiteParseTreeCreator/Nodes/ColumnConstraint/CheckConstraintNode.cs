using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint
{
    public class CheckConstraintNode : ColumnConstraintNode
    {
        public CheckConstraintNode()
        {

        }

        public CheckConstraintNode(ParserRuleContext context) : base(context)
        {

        }
        
        public string Expr { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
