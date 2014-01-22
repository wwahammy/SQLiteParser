// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes
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
