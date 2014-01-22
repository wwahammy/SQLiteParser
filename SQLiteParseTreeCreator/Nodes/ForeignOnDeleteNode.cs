// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes
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
