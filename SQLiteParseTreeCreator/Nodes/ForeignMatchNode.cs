using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace SQLiteParseTreeCreator.Nodes
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
