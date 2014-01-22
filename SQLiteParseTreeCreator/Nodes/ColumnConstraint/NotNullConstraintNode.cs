// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint
{
    public class NotNullConstraintNode : ColumnConstraintNode
    {
        public NotNullConstraintNode()
        {
            
        }

        public NotNullConstraintNode(ParserRuleContext context) : base(context)
        {
            
        }

        public ConflictClauseNode ConflictClause { get; set; }


        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
           return visitor.Visit(this);
        }
    }
}
