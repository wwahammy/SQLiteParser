using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.TableConstraint
{
    public class IndexedColumnNode : SQLiteParseTreeNode
    {
        public IndexedColumnNode()
        {

        }

        public IndexedColumnNode(ParserRuleContext context) : base(context)
        {

        }

        public string Id { get; set; }

        public string CollationId { get; set; }
        public SortOrder? Order { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
