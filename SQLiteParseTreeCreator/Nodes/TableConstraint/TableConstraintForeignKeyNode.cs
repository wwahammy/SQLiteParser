using System.Collections.Generic;
using Antlr4.Runtime;
using SQLiteParseTreeCreator.Nodes.ColumnConstraint;

namespace SQLiteParseTreeCreator.Nodes.TableConstraint
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
