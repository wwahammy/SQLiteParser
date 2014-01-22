// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System;
using System.Data;

namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public class AlterTableCommand : SchemaCommand {
        public AlterTableCommand(string name)
            : base(name) {
        }

        public void AddColumn(string columnName, string dbType, Action<AddColumnCommand> column = null) {
            var command = new AddColumnCommand(Name, columnName);
            command.WithType(dbType);
            
            if(column != null) {
                column(command);
            }
            
            TableCommands.Add(command);
        }
      

        public void DropColumn(string columnName) {
            var command = new DropColumnCommand(Name, columnName);
            TableCommands.Add(command);
        }

        public void AlterColumn(string columnName, Action<AlterColumnCommand> column = null) {
            var command = new AlterColumnCommand(Name, columnName);

            if ( column != null ) {
                column(command);
            }

            TableCommands.Add(command);
        }

        public void CreateIndex(string indexName, params string[] columnNames) {
            var command = new AddIndexCommand(Name, indexName, columnNames);
            TableCommands.Add(command);
        }

        public void DropIndex(string indexName) {
            var command = new DropIndexCommand(Name, indexName);
            TableCommands.Add(command);
        }

        public void DropForeignKey(string key)
        {
            var command = new DropForeignKeyCommand(Name, key);
            TableCommands.Add(command);
        }

        public void CreateForeignKey(string key, string[] srcColumns, string destTable, string[] destColumns)
        {
            var command = new CreateForeignKeyCommand(key, Name, srcColumns, destTable, destColumns);
            TableCommands.Add(command);
        }
    }
}
