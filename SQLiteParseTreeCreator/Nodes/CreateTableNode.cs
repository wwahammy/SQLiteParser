using System.Collections.Generic;
using Outercurve.SQLiteParser;
using SQLiteParseTreeCreator.Nodes.TableConstraint;

namespace SQLiteParseTreeCreator.Nodes
{
    public class CreateTableNode : SQLiteParseTreeNode
    {


        public CreateTableNode()
        {
            
        }

        public CreateTableNode(SQLiteParserSimpleParser.Create_table_stmtContext context) : base(context)
        {
   
        }

        public bool HasSelectStmt
        {
            get
            {
                return SelectStmt != null;
            }
        }

        public string DatabaseName { get; set; }

        public string TableName { get; set; }

      //  public QualifiedNameNode TableName { get; set; }

        public bool Temp { get; set; }

        public IList<ColumnDefNode> ColumnDefinitions { get; set; }
        public IList<TableConstraintNode> TableConstraints { get; set; }

        public bool IfNotExists { get; set; }

        public bool WithoutRowId { get; set; }

        public string SelectStmt { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
