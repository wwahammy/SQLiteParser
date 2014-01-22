// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
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
