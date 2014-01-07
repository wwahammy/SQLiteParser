using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes
{
    public class ForeignOnUpdateNode : SQLiteParseTreeNode
    {
        public ForeignOnUpdateNode()
        {

        }

        public ForeignOnUpdateNode(ParserRuleContext context) : base(context)
        {

        }

        public ForeignDeleteOrUpdateAction Action { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
