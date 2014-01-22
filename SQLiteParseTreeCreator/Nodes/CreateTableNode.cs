// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System.Collections.Generic;
using Outercurve.SQLiteCreateTree.Nodes.TableConstraint;
using Outercurve.SQLiteParser;

namespace Outercurve.SQLiteCreateTree.Nodes
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
