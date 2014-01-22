// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes
{
    public class ForeignDeferrableNode : SQLiteParseTreeNode
    {
        public ForeignDeferrableNode()
        {

        }

        public ForeignDeferrableNode(ParserRuleContext context) : base(context)
        {

        }


        public ForeignDeferrableNode SetToTrulyDeferrable()
        {
            IsDeferrable = true;
            InitiallyImmediate = false;
            return this;
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
