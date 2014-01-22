// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.


namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public class DropForeignKeyCommand : TableCommand {
        public string Name { get; private set; }

        public DropForeignKeyCommand(string srcTable, string name)
            : base(srcTable)
        {
            Name = name;
        }

        
    }
}
