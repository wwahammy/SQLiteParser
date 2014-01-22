// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree.Nodes.TableConstraint
{
    public abstract class TableConstraintNode : SQLiteParseTreeNode
    {
        protected TableConstraintNode()
        {
        }

        protected TableConstraintNode(ParserRuleContext context) : base(context)
        {
            
        }

        public string ConstraintName { get; set; }

    }
}
