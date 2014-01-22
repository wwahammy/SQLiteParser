// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System.Collections.Generic;
using Outercurve.SQLiteParser;

namespace Outercurve.SQLiteCreateTree.Nodes
{
    public class TypeNameNode : SQLiteParseTreeNode
    {
        private IList<string> _signedNumbers = new List<string>();

        public TypeNameNode()
        {
            
        }
        public TypeNameNode(SQLiteParserSimpleParser.Type_nameContext context) : base(context)
        {
            
        }
        public string TypeName { get; set; }

        public IList<string> SignedNumbers
        {
            get { return _signedNumbers; }
            set { _signedNumbers = value; }
        }

        public override TResult Accept<TResult>(ILogicalParseTreeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
