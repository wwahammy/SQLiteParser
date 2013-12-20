using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using SQLiteParseTreeCreator.Nodes;

namespace SQLiteParseTreeCreator
{
    public abstract class SQLiteParseTreeNode
    {
        protected SQLiteParseTreeNode()
        {
        }


        protected SQLiteParseTreeNode(ParserRuleContext context)
        {
            Context = context;
        }

        protected SQLiteParseTreeNode(SQLiteParseTreeNode parent)
        {
            Parent = parent;
        }

        public ParserRuleContext Context { get; private set; }
        public SQLiteParseTreeNode Parent { get; private set; }


        public abstract TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor);
    }
}
