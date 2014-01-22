// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint
{
    public class DefaultConstraintNode : ColumnConstraintNode
    {
        public DefaultConstraintNode()
        {

        }

        public DefaultConstraintNode(ParserRuleContext context) : base(context)
        {

        }

        //do we really need this?
        public string Value { get; set; }
        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
