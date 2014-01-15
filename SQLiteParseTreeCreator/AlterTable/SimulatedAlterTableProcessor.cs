using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outercurve.SQLiteCreateTree.AlterTable.Action;
using Outercurve.SQLiteCreateTree.Nodes;
using Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint;
using Outercurve.SQLiteCreateTree.Nodes.TableConstraint;

namespace Outercurve.SQLiteCreateTree.AlterTable
{
    public class SimulatedAlterTableProcessor
    {
        public SimulatedAlterTableProcessor(CreateTableNode node, IEnumerable<CreateIndexNode> indexNodes)
        {
            CreateTableNode = node;
            CreateIndexNodes = indexNodes.ToList();
        }
        public CreateTableNode CreateTableNode { get; private set; }
        public IList<CreateIndexNode> CreateIndexNodes { get; private set; }

        //we need to do the work around. http://www.sqlite.org/faq.html#q11
        public  IEnumerable<string> CreateSqlStatements(AlterTableCommand command)
        {
            string fullTableName = command.Name;
            string tempTableName = fullTableName + "_" + Guid.NewGuid().ToString("N");

            
        

            yield return CreateTempTable(tempTableName, fullTableName);
            
            
            //modify all the references to the old table to go to the new table

            //drop the old table
            yield return DropTable(fullTableName);

            //create newTable and insert the data, add and drop indices
            foreach (string statement in CreateNewTableCopyInDataAndCreateIndices(command, tempTableName))
            {
                yield return statement;
            }
            
            //change them all back!

            //drop the tempTable
            yield return DropTable(tempTableName);
            
        }

        private string CreateTempTable(string tempTable, string originalTable)
        {
            return String.Format("CREATE TEMPORARY TABLE {0} AS SELECT * FROM {1};", tempTable, originalTable);
        }

        private string DropTable(string tableName)
        {
            return String.Format("DROP TABLE {0};", tableName);
        }

        private string SavePoint(out string savepoint)
        {
            savepoint = "s_" + Guid.NewGuid().ToString("N");
            return String.Format("SAVEPOINT {0};", savepoint);
        }

        private string ReleaseSavepoint(string savepoint)
        {
            return String.Format("RELEASE {0};", savepoint);
        }
        private string TurnOffReferentialIntegrity()
        {
            return "PRAGMA defer_foreign_keys = 1;";
        }

        private string TurnOnReferentialIntegrity()
        {
            return "PRAGMA defer_foreign_keys = 0;";
        }

        private string InsertInto(string toTable, IEnumerable<string> columnNames, string fromTable)
        {
            return String.Format("INSERT INTO {0} ({1}) SELECT {1} FROM {2};", toTable, string.Join(",", columnNames), fromTable);
        }

        private IEnumerable<string> CreateNewTableCopyInDataAndCreateIndices(AlterTableCommand command, string tempTable)
        {

            string fullTableName = command.Name;
            
            //we'll use this to create the insert into later
            var originalColumns = CreateTableNode.ColumnDefinitions.Select(i => i.ColumnName).ToArray();

            CreateTableNode = IncorporateAlterationsInCreateNode(command, CreateTableNode);

            var visitor = new TreeStringOutputVisitor();

            yield return CreateTableNode.Accept(visitor).ToString();
            var finalSetOfColumns = originalColumns.Where(i => command.TableCommands.OfType<DropColumnCommand>().All(j => j.ColumnName != i)).ToArray();

            // this can only be the ones in the ORIGINAL table that we want to copy, i.e. don't copy in deleted columns
            yield return InsertInto(fullTableName, finalSetOfColumns, tempTable);

            //let's figure out our final indexes
            IncorporateAlterationsInIndexNodes(CreateIndexNodes, fullTableName, command.TableCommands, finalSetOfColumns);
            var indexNodeVisitor = new TreeStringOutputVisitor();
            foreach (var indexNode in CreateIndexNodes)
            {
                yield return indexNode.Accept(indexNodeVisitor).ToString();
            }

        }

        private void IncorporateAlterationsInIndexNodes(IList<CreateIndexNode> indexes, string fullTableName, IEnumerable<TableCommand> commands, IEnumerable<string> finalColumnNames)
        {
            //let's handle the commands first
            foreach (var command in commands)
            {
                if (command is DropIndexCommand)
                {
                    var drop = command as DropIndexCommand;
                    var node = indexes.FirstOrDefault(i => i.IndexName == drop.IndexName);
                    if (node == null)
                    {
                        throw new Exception(string.Format("The index '{0}' does not exist and cannot be dropped.", drop.IndexName));
                    }
                    indexes.Remove(node);
                }
                else if (command is AddIndexCommand)
                {
                    var add = command as AddIndexCommand;
                    indexes.Add(CreateIndexNodeFromAddColumnCommand(add, fullTableName));
                }
            }


            //we need to remove all indexes that have columns that aren't in finalColumnNames
            //hashset to speed this up a little
            var finalColumnHash = new HashSet<string>(finalColumnNames);
            RemoveAll(indexes, node => !node.IndexedColumnNodes.All(i => finalColumnHash.Contains(i.Id)));
        }





        private void RemoveAll<T>(IList<T> items, Predicate<T> predicate)
        {
            var itemsToRemove = items.Where(i => predicate(i)).ToList();
            foreach(var itemToRemove in itemsToRemove)
                items.Remove(itemToRemove);
        }
       

        private CreateTableNode IncorporateAlterationsInCreateNode(AlterTableCommand command, CreateTableNode createTableNode)
        {
            foreach (TableCommand alterCommand in command.TableCommands)
            {
                if (alterCommand is AddColumnCommand)
                {
                    var addColumn = alterCommand as AddColumnCommand;
                    createTableNode.ColumnDefinitions.Add(AlterTableAdapter.CreateColumnNode(addColumn));
                }
                else if (alterCommand is DropColumnCommand)
                {
                    var dropColumn = alterCommand as DropColumnCommand;

                    ColumnDefNode result = createTableNode.ColumnDefinitions.FirstOrDefault(i => i.ColumnName == dropColumn.ColumnName);
                    if (result == null)
                    {
                        //bad!!
                    }
                    else
                    {
                        //remove our column
                        createTableNode.ColumnDefinitions.Remove(result);
                    }
                }
                else if (alterCommand is AlterColumnCommand)
                {
                    var alterColumn = alterCommand as AlterColumnCommand;
                    ColumnDefNode columnDef = createTableNode.ColumnDefinitions.FirstOrDefault(i => i.ColumnName == alterColumn.ColumnName);

                    if (columnDef == null)
                    {
                        //throw!!!!
                    }
                    //modify the type name
                    columnDef.TypeNameNode = AlterTableAdapter.CreateTypeNameNode(alterColumn.DbType);

                    //modify the default
                    DefaultConstraintNode defaultConstraint = columnDef.ColumnConstraints.OfType<DefaultConstraintNode>().FirstOrDefault();
                    if (defaultConstraint == null)
                    {
                        //we'll create our own

                        defaultConstraint = new DefaultConstraintNode();
                        defaultConstraint.Value = AlterTableAdapter.ConvertToSqlValue(alterColumn.Default);

                        //and add it!
                        columnDef.ColumnConstraints.Add(defaultConstraint);
                    }
                    else
                    {
                        //we modify the one that exists

                        defaultConstraint.Value = AlterTableAdapter.ConvertToSqlValue(alterColumn.Default);
                    }
                }
                else if (alterCommand is CreateForeignKeyCommand)
                {
                    var foreignKeyCommand = alterCommand as CreateForeignKeyCommand;

                    var keyNode = new TableConstraintForeignKeyNode();
                    keyNode.FieldNames = foreignKeyCommand.SrcColumns;
                    keyNode.ConstraintName = foreignKeyCommand.Name;
                    keyNode.ForeignKeyClauseNode = new ForeignKeyClauseNode()
                    {
                        TableName = foreignKeyCommand.DestTable,
                        FieldList = foreignKeyCommand.DestColumns,
                        ForeignDeferrable = new ForeignDeferrableNode().SetToTrulyDeferrable()
                    };
                    
                    createTableNode.TableConstraints.Add(keyNode);
                }
                else if (alterCommand is DropForeignKeyCommand)
                {
                    var foreignKeyCommand = alterCommand as DropForeignKeyCommand;

                    var foreignKeyDrop = createTableNode.TableConstraints.OfType<TableConstraintForeignKeyNode>()
                        .FirstOrDefault(n => n.ConstraintName  == foreignKeyCommand.Name);

                    if (foreignKeyDrop == null)
                    {
                        throw new Exception("No foreign key exists.");
                    }
                    createTableNode.TableConstraints.Remove(foreignKeyDrop);

                }
            }
            return createTableNode;
        }


        private CreateIndexNode CreateIndexNodeFromAddColumnCommand(AddIndexCommand command, string fullTableName)
        {
            var ret = new CreateIndexNode { TableName = fullTableName, IndexName = command.IndexName, IndexedColumnNodes = command.ColumnNames.Select(i => new IndexedColumnNode { Id = i }) };
            return ret;
        }
    }
}
