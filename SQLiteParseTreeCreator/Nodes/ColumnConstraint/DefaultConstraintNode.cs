using Antlr4.Runtime;

namespace SQLiteParseTreeCreator.Nodes.ColumnConstraint
{
    public class DefaultConstraintNode : ColumnConstraintNode
    {
        public DefaultConstraintNode()
        {

        }

        public DefaultConstraintNode(ParserRuleContext context) : base(context)
        {

        }

        //do we really need this?
        public string Value { get; set; }
        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
