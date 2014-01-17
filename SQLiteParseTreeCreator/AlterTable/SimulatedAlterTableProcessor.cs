using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<string> CreateSqlStatements(AlterTableCommand command)
        {
            string fullTableName = command.Name;
            string tempTableName = fullTableName + "_" + Guid.NewGuid().ToString("N");


            yield return StatementUtil.CreateTempTable(tempTableName, fullTableName);


            //modify all the references to the old table to go to the new table

            //drop the old table
            yield return StatementUtil.DropTable(fullTableName);

            //create newTable and insert the data, add and drop indices
            foreach (string statement in CreateNewTableCopyInDataAndCreateIndices(command, tempTableName))
            {
                yield return statement;
            }

            //change them all back!

            //drop the tempTable
            yield return StatementUtil.DropTable(tempTableName);
        }


        private IEnumerable<string> CreateNewTableCopyInDataAndCreateIndices(AlterTableCommand command, string tempTable)
        {
            string fullTableName = command.Name;

            //we'll use this to create the insert into later
            string[] originalColumns = CreateTableNode.ColumnDefinitions.Select(i => i.ColumnName).ToArray();

            CreateTableNode = IncorporateAlterationsInCreateNode(command, CreateTableNode);

            var visitor = new TreeStringOutputVisitor();

            yield return CreateTableNode.Accept(visitor).ToString();
            string[] finalSetOfColumns =
                originalColumns.Where(i => command.TableCommands.OfType<DropColumnCommand>().All(j => j.ColumnName != i))
                    .ToArray();

            // this can only be the ones in the ORIGINAL table that we want to copy, i.e. don't copy in deleted columns
            yield return StatementUtil.InsertInto(fullTableName, finalSetOfColumns, tempTable);

            //let's figure out our final indexes
            IncorporateAlterationsInIndexNodes(CreateIndexNodes, fullTableName, command.TableCommands, finalSetOfColumns);
            var indexNodeVisitor = new TreeStringOutputVisitor();
            foreach (CreateIndexNode indexNode in CreateIndexNodes)
            {
                yield return indexNode.Accept(indexNodeVisitor).ToString();
            }
        }

        private void IncorporateAlterationsInIndexNodes(IList<CreateIndexNode> indexes, string fullTableName,
            IEnumerable<TableCommand> commands, IEnumerable<string> finalColumnNames)
        {
            //let's handle the commands first
            foreach (TableCommand command in commands)
            {
                if (command is DropIndexCommand)
                {
                    var drop = command as DropIndexCommand;
                    CreateIndexNode node = indexes.FirstOrDefault(i => i.IndexName == drop.IndexName);
                    if (node == null)
                    {
                        throw new InvalidIndexException(string.Format("The index '{0}' on table '{1}' does not exist and cannot be dropped.",
                            drop.IndexName, drop.TableName), drop);
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
            List<T> itemsToRemove = items.Where(i => predicate(i)).ToList();
            foreach (T itemToRemove in itemsToRemove)
                items.Remove(itemToRemove);
        }


        private CreateTableNode IncorporateAlterationsInCreateNode(AlterTableCommand command,
            CreateTableNode createTableNode)
        {
            foreach (TableCommand alterCommand in command.TableCommands)
            {
                if (alterCommand is AddColumnCommand)
                {
                    var addColumn = alterCommand as AddColumnCommand;
                    createTableNode.ColumnDefinitions.Add(addColumn.CreateColumnDefNode());
                }
                else if (alterCommand is DropColumnCommand)
                {
                    var dropColumn = alterCommand as DropColumnCommand;

                    ColumnDefNode result =
                        createTableNode.ColumnDefinitions.FirstOrDefault(i => i.ColumnName == dropColumn.ColumnName);
                    if (result == null)
                    {
                        //bad!!
                        throw new InvalidColumnException<DropColumnCommand>(String.Format("Altering column {0} failed. No such column exists on table {1}.",dropColumn.ColumnName, dropColumn.TableName), dropColumn);
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
                    ColumnDefNode columnDef =
                        createTableNode.ColumnDefinitions.FirstOrDefault(i => i.ColumnName == alterColumn.ColumnName);

                    if (columnDef == null)
                    {
                        //throw!!!!
                        throw new InvalidColumnException<AlterColumnCommand>(String.Format("Altering column {0} failed. No such column exists on table {1}.",alterColumn.ColumnName, alterColumn.TableName), alterColumn);
                    }
                    //modify the type name
                    if (!String.IsNullOrEmpty(alterColumn.DbType))
                    {

                        columnDef.TypeNameNode = SQLiteParseVisitor.ParseString<TypeNameNode>(alterColumn.DbType,
                            i => i.type_name());
                    }

                    if (alterColumn.Default != null)
                    {
                        //modify the default
                        DefaultConstraintNode defaultConstraint =
                            columnDef.ColumnConstraints.OfType<DefaultConstraintNode>().FirstOrDefault();
                        if (defaultConstraint == null)
                        {
                            //we'll create our own

                            defaultConstraint = new DefaultConstraintNode
                            {
                                Value = DbUtils.ConvertToSqlValue(alterColumn.Default)
                            };

                            //and add it!
                            columnDef.ColumnConstraints.Add(defaultConstraint);
                        }
                        else
                        {
                            //we modify the one that exists

                            defaultConstraint.Value = DbUtils.ConvertToSqlValue(alterColumn.Default);
                        }
                    }
                }
                else if (alterCommand is CreateForeignKeyCommand)
                {
                    var foreignKeyCommand = alterCommand as CreateForeignKeyCommand;

                    var keyNode = new TableConstraintForeignKeyNode
                    {
                        FieldNames = foreignKeyCommand.SrcColumns,
                        ConstraintName = foreignKeyCommand.Name,
                        ForeignKeyClauseNode = new ForeignKeyClauseNode
                        {
                            TableName = foreignKeyCommand.DestTable,
                            FieldList = foreignKeyCommand.DestColumns,
                            ForeignDeferrable = new ForeignDeferrableNode().SetToTrulyDeferrable()
                        }
                    };

                    createTableNode.TableConstraints.Add(keyNode);
                }
                else if (alterCommand is DropForeignKeyCommand)
                {
                    var foreignKeyCommand = alterCommand as DropForeignKeyCommand;

                    TableConstraintForeignKeyNode foreignKeyDrop = createTableNode.TableConstraints
                        .OfType<TableConstraintForeignKeyNode>()
                        .FirstOrDefault(n => n.ConstraintName == foreignKeyCommand.Name);

                    if (foreignKeyDrop == null)
                    {
                        throw new InvalidForeignKeyException(String.Format("No foreign key {0} exists.", foreignKeyCommand.Name), foreignKeyCommand);
                    }
                    createTableNode.TableConstraints.Remove(foreignKeyDrop);
                }
            }
            return createTableNode;
        }


        private CreateIndexNode CreateIndexNodeFromAddColumnCommand(AddIndexCommand command, string fullTableName)
        {
            var ret = new CreateIndexNode
            {
                TableName = fullTableName,
                IndexName = command.IndexName,
                IndexedColumnNodes = command.ColumnNames.Select(i => new IndexedColumnNode {Id = i})
            };
            return ret;
        }
    }
}