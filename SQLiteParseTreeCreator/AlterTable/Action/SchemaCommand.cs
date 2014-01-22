// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System.Collections.Generic;

namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public abstract class SchemaCommand : ISchemaBuilderCommand {
        protected SchemaCommand(string name ) {
            TableCommands = new List<TableCommand>();
            WithName(name);
        }

        public string Name { get; private set; }
        public List<TableCommand> TableCommands { get; private set; }


        public SchemaCommand WithName(string name) {
            Name = name;
            return this;
        }
    }

    
}
