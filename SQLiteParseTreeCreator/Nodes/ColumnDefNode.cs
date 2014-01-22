// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System.Collections.Generic;
using Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint;
using Outercurve.SQLiteParser;

namespace Outercurve.SQLiteCreateTree.Nodes
{
    public class ColumnDefNode : SQLiteParseTreeNode
    {
        private IList<ColumnConstraintNode> _columnConstraintNodes = new List<ColumnConstraintNode>();
        public ColumnDefNode()
        {
            
        }
        public ColumnDefNode( SQLiteParserSimpleParser.Column_defContext context  ) : base(context)
        {

        }

        public string ColumnName { get; set; }
        public TypeNameNode TypeNameNode { get; set; }

        public IList<ColumnConstraintNode> ColumnConstraints
        {
            get
            {
                return _columnConstraintNodes;
            }
            set
            {
                _columnConstraintNodes = value;
            }
        }
        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

    }
}
