using Antlr4.Runtime;

namespace Outercurve.SQLiteCreateTree
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
