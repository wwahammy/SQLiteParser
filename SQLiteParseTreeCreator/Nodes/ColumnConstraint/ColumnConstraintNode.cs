// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint
{
    public abstract class ColumnConstraintNode :SQLiteParseTreeNode
    {
        protected ColumnConstraintNode()
        {
            
        }

        protected ColumnConstraintNode(ParserRuleContext context)   : base(context)
        {
            
        }

        public string Name { get; set; }

    }
}
