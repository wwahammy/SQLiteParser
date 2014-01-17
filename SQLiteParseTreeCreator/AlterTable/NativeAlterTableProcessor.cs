using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outercurve.SQLiteCreateTree.AlterTable.Action;

namespace Outercurve.SQLiteCreateTree.AlterTable
{
    public class NativeAlterTableProcessor
    {
        /// <summary>
        /// returns null if something if we don't have ones we can do natively
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public IEnumerable<string> CreateSqlStatements(AlterTableCommand command)
        {
            if (!command.TableCommands.All(i => i is AddColumnCommand || i is AddIndexCommand || i is DropIndexCommand))
            {
                // we have non Native Alterations to make
                yield break;
            }
            var visitor = new TreeStringOutputVisitor();

            foreach (var alterCommand in command.TableCommands)
            {
                if (alterCommand is AddColumnCommand)
                {
                    var addColumn = alterCommand as AddColumnCommand;
                    yield return
                        StatementUtil.AddColumn(command.Name,
                            addColumn.CreateColumnDefNode().Accept(visitor).ToString());
                }
                else if (alterCommand is AddIndexCommand)
                {
                    var addIndexCommand = alterCommand as AddIndexCommand;

                    yield return StatementUtil.AddIndex(addIndexCommand.IndexName, addIndexCommand.TableName, addIndexCommand.ColumnNames);
                }
                else if (alterCommand is DropIndexCommand)
                {
                    var dropIndexCommand = alterCommand as DropIndexCommand;
                    yield return StatementUtil.DropIndex(dropIndexCommand.IndexName);
                }
            }
        }
    }
}
