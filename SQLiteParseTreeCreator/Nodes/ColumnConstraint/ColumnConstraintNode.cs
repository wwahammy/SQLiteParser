using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint
{
    public abstract class ColumnConstraintNode :SQLiteParseTreeNode
    {
        protected ColumnConstraintNode()
        {
            
        }

        protected ColumnConstraintNode(ParserRuleContext context)   : base(context)
        {
            
        }

        public string Name { get; set; }

    }
}
