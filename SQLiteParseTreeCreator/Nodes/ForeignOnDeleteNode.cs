using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace SQLiteParseTreeCreator.Nodes
{
    public class ForeignOnDeleteNode : SQLiteParseTreeNode
    {
        public ForeignOnDeleteNode()
        {

        }

        public ForeignOnDeleteNode(ParserRuleContext context) : base(context)
        {

        }

        public ForeignDeleteOrUpdateAction Action { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
