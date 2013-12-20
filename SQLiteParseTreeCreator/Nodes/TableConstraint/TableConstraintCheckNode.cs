using Antlr4.Runtime;

namespace SQLiteParseTreeCreator.Nodes.TableConstraint
{
    public class TableConstraintCheckNode : TableConstraintNode
    {
        public TableConstraintCheckNode()
        {

        }

        public TableConstraintCheckNode(ParserRuleContext context) : base(context)
        {

        }

        public string Expr { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
