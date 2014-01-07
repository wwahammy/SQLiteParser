using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes
{
    public class ForeignMatchNode : SQLiteParseTreeNode
    {
        public ForeignMatchNode()
        {

        }

        public ForeignMatchNode(ParserRuleContext context) : base(context)
        {

        }

        public string Id { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
