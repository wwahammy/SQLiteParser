using System.Collections.Generic;
using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint
{
    public class ForeignKeyClauseNode : ColumnConstraintNode
    {
        public ForeignKeyClauseNode()
        {

        }

        public ForeignKeyClauseNode(ParserRuleContext context) : base(context)
        {

        }

        public string TableName { get; set; }

        public IList<string> FieldList { get; set; }

        public ForeignDeferrableNode ForeignDeferrable { get; set; }
        public IList<ForeignOnDeleteNode> ForeignOnDelete { get; set; }
        public IList<ForeignOnUpdateNode> ForeignOnUpdate { get; set; }
        public IList<ForeignMatchNode> ForeignMatch { get; set; }


        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
