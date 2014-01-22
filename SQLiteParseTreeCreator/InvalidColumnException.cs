// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outercurve.SQLiteCreateTree.AlterTable.Action;

namespace Outercurve.SQLiteCreateTree
{
    public class InvalidColumnException<T> : Exception where T : ColumnCommand
    {
        public T Command { get; private set; }

        public InvalidColumnException(string message) : base(message)
        {
            
        }

        public InvalidColumnException(string message, T command) : base(message)
        {
            Command = command;
        }
    }
}
