using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace SQLiteParseTreeCreator.Nodes.TableConstraint
{
    public abstract class TableConstraintNode : SQLiteParseTreeNode
    {
        protected TableConstraintNode()
        {
        }

        protected TableConstraintNode(ParserRuleContext context) : base(context)
        {
            
        }

        public string ConstraintName { get; set; }

    }
}
