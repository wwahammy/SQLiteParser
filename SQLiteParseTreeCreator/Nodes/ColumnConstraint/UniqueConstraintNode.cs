// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint
{
    public class UniqueConstraintNode : ColumnConstraintNode
    {
        public UniqueConstraintNode()
        {

        }

        public UniqueConstraintNode(ParserRuleContext context) : base(context)
        {

        }

        /// <summary>
        /// Conflict Clause is really weird, it can be empty. We treat that as null.
        /// </summary>
        public ConflictClauseNode ConflictClause { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
