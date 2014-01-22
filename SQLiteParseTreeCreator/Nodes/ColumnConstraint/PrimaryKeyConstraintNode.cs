// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint
{
    public class PrimaryKeyConstraintNode : ColumnConstraintNode
    {
        public PrimaryKeyConstraintNode()
        {

        }

        public PrimaryKeyConstraintNode(ParserRuleContext context) : base(context)
        {

        }

        public SortOrder? Order { get; set; }

        public ConflictClauseNode ConflictClause { get; set; }

        public bool AutoIncrement { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
