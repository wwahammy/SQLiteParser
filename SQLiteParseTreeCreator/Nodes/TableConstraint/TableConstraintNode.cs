using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.TableConstraint
{
    public abstract class TableConstraintNode : SQLiteParseTreeNode
    {
        protected TableConstraintNode()
        {
        }

        protected TableConstraintNode(ParserRuleContext context) : base(context)
        {
            
        }

        public string ConstraintName { get; set; }

    }
}
