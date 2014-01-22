// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Outercurve.SQLiteCreateTree.Nodes.TableConstraint;

namespace Outercurve.SQLiteCreateTree.Nodes
{
    public class CreateIndexNode : SQLiteParseTreeNode
    {
        public CreateIndexNode ()
        {

        }

        public CreateIndexNode(ParserRuleContext context)
             : base(context)
        {

        }

        public bool IsUnique { get; set; }

        public bool IfNotExists { get; set; }

        public string DatabaseName { get; set; }

        public string IndexName { get; set; }

        public string TableName { get; set; }

        public IEnumerable<IndexedColumnNode> IndexedColumnNodes { get; set; }

        public string WhereExpr { get; set; }


        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
