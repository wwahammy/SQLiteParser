using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outercurve.SQLiteParser;

namespace SQLiteParseTreeCreator.Nodes
{
    public class TypeNameNode : SQLiteParseTreeNode
    {
        public TypeNameNode()
        {
            
        }
        public TypeNameNode(SQLiteParserSimpleParser.Type_nameContext context) : base(context)
        {
            
        }
        public string TypeName { get; set; }

        public IList<string> SignedNumbers { get; set; }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
