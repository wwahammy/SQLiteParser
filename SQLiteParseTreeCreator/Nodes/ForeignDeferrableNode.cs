using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace SQLiteParseTreeCreator.Nodes
{
    public class ForeignDeferrableNode : SQLiteParseTreeNode
    {
        public ForeignDeferrableNode()
        {

        }

        public ForeignDeferrableNode(ParserRuleContext context) : base(context)
        {

        }


        /// <summary>
        /// NOT? 
        /// </summary>
        public bool IsDeferrable { get; set; }


       
        /// <summary>
        /// ((INITIALLY DEFERRED)| (INITIALLY IMMEDIATE))?
        /// </summary>
        public bool? InitiallyImmediate { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
