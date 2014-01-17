using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outercurve.SQLiteCreateTree
{
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message)
        {
            
        }
    }
}
