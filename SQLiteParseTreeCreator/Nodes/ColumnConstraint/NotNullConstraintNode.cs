using Antlr4.Runtime;

namespace SQLiteParseTreeCreator.Nodes.ColumnConstraint
{
    public class NotNullConstraintNode : ColumnConstraintNode
    {
        public NotNullConstraintNode()
        {
            
        }

        public NotNullConstraintNode(ParserRuleContext context) : base(context)
        {
            
        }

        public ConflictClauseNode ConflictClause { get; set; }


        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
           return visitor.Visit(this);
        }
    }
}
