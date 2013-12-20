using System.Collections;
using System.Collections.Generic;
using Outercurve.SQLiteParser;
using SQLiteParseTreeCreator.Nodes.ColumnConstraint;

namespace SQLiteParseTreeCreator.Nodes
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
