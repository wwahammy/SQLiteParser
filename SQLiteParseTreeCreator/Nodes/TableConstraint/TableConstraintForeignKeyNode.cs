// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System.Collections.Generic;
using Antlr4.Runtime;
using Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint;

namespace Outercurve.SQLiteCreateTree.Nodes.TableConstraint
{
    public class TableConstraintForeignKeyNode : TableConstraintNode
    {
        public TableConstraintForeignKeyNode()
        {

        }

        public TableConstraintForeignKeyNode(ParserRuleContext context) : base(context)
        {

        }

        public IList<string> FieldNames { get; set; }

        public ForeignKeyClauseNode ForeignKeyClauseNode { get; set; }
       

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
