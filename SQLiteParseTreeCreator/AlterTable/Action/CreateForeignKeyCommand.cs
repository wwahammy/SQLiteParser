// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public class CreateForeignKeyCommand : TableCommand {

        public string[] DestColumns { get; private set; }

        public string DestTable { get; private set; }

        public string[] SrcColumns { get; private set; }
        public string Name { get; private set; }



        public CreateForeignKeyCommand(string name, string srcTable, string[] srcColumns, string destTable, string[] destColumns) : base(srcTable) {
            SrcColumns = srcColumns;
            DestTable = destTable;
            DestColumns = destColumns;
            Name = name;
        }
    }
}
