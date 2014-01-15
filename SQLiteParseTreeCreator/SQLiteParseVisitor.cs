using System;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Outercurve.SQLiteCreateTree.Nodes;
using Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint;
using Outercurve.SQLiteCreateTree.Nodes.TableConstraint;
using Outercurve.SQLiteParser;

namespace Outercurve.SQLiteCreateTree
{
    public class SQLiteParseVisitor : SQLiteParserSimpleBaseVisitor<SQLiteParseTreeNode>
    {

        public static SQLiteParseTreeNode ParseString(string queryString,
            Func<SQLiteParserSimpleParser, IParseTree> startingNode)
        {
            var inputStream = new AntlrInputStream(queryString);
            var sqliteLexer = new SQLiteParserSimpleLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(sqliteLexer);
            var sqliteParser = new SQLiteParserSimpleParser(commonTokenStream); 
            var visitor = new SQLiteParseVisitor();
            return visitor.Visit(startingNode(sqliteParser));
        }

        public static SQLiteParseTreeNode ParseString(string queryString)
        {
            return ParseString(queryString, i => i.sql_stmt());
        }

        public static T ParseString<T>(string queryString, Func<SQLiteParserSimpleParser, IParseTree> startingNode) where T : SQLiteParseTreeNode
        {
             return ParseString(queryString, startingNode) as T;
        }

        public static T ParseString<T>(string queryString) where T : SQLiteParseTreeNode
        {
            return ParseString<T>(queryString, i => i.sql_stmt());
        }

        public override SQLiteParseTreeNode VisitSql_stmt(SQLiteParserSimpleParser.Sql_stmtContext context)
        {
            
            if (context.create_table_stmt() != null)
            {
                return context.create_table_stmt().Accept(this);
            }
            else if (context.create_index_stmt() != null)
            {
                return context.create_index_stmt().Accept(this);
            }
            throw new Exception("It's not a statement we can handle");
        }
        public override SQLiteParseTreeNode VisitCreate_table_stmt(SQLiteParserSimpleParser.Create_table_stmtContext context)
        {
            var ret = new CreateTableNode(context)
            {
                DatabaseName = context.database_name() == null ? null :  context.database_name().GetText(),
                TableName = context.table_name().GetText(),
                Temp = context.TEMPORARY() != null,
                IfNotExists = context.IF() != null
            };
            if (context.AS() != null)
            {
                //it's a select stmt
                ret.SelectStmt = context.select_stmt().GetText();
            }
            else
            {
                ret.ColumnDefinitions = context.column_def().Select(i => i.Accept(this) as ColumnDefNode).ToList();

                ret.TableConstraints =
                    context.table_constraint().Select(i => i.Accept(this) as TableConstraintNode).ToList();
                ret.WithoutRowId = context.WITHOUT() != null;

            }
            

            return ret;
        }

        public override SQLiteParseTreeNode VisitCreate_index_stmt(SQLiteParserSimpleParser.Create_index_stmtContext context)
        {
            var ret = new CreateIndexNode(context)
            {
                TableName = context.table_name().GetText(),
                IndexName = context.index_name().GetText(),
                IsUnique = context.UNIQUE() != null,
                IfNotExists = context.EXISTS() != null,
                DatabaseName = context.database_name() != null ? context.database_name().GetText() : null,
                WhereExpr = context.expr() != null ? context.expr().GetText() : null,
                IndexedColumnNodes = context.indexed_column().Select(CreateIndexedColumnNode).ToArray()
            };
            return ret;

        }

        public override SQLiteParseTreeNode VisitColumn_def(SQLiteParserSimpleParser.Column_defContext context)
        {
            var ret = new ColumnDefNode(context) {ColumnName = context.name().GetText()};

            var typeName = context.type_name();
            if (typeName != null)
            {
                ret.TypeNameNode = typeName.Accept(this) as TypeNameNode;
            }

            ret.ColumnConstraints = context.column_constraint().Select(c => c.Accept(this)).Cast<ColumnConstraintNode>().ToList();

            return ret;

        }

        public override SQLiteParseTreeNode VisitTable_constraint(SQLiteParserSimpleParser.Table_constraintContext context)
        {
            TableConstraintNode ret = null;

            if (context.table_constraint__index_clause() != null)
            {
                ret =  context.table_constraint__index_clause().Accept(this) as TableConstraintNode;
            }
            else if (context.table_constraint__check() != null)
            {
                ret = context.table_constraint__check().Accept(this) as TableConstraintNode;
            }
            else
            {
                ret =context.table_constraint__foreign_key_constraint().Accept(this) as TableConstraintNode;
            }

            if (context.name() != null && ret != null)
            {
                ret.ConstraintName = context.name().GetText();
            }

            return ret;
        }

        public override SQLiteParseTreeNode VisitType_name(SQLiteParserSimpleParser.Type_nameContext context)
        {
            var ret = new TypeNameNode(context)
            {
                TypeName = context.ID().GetText(), 
                SignedNumbers = context.signed_number().Select( i => i.GetText()).ToList()
            };
            return ret;
        }


        public override SQLiteParseTreeNode VisitTable_constraint__check(SQLiteParserSimpleParser.Table_constraint__checkContext context)
        {
            var ret = new TableConstraintCheckNode(context) {Expr = context.expr().GetText()};
            return ret;
        }
        public override SQLiteParseTreeNode VisitConflict_clause(SQLiteParserSimpleParser.Conflict_clauseContext context)
        {
            //this is empty, which is possible in this strange rule
            if (context.ON() == null)
            {
                return null;
            }

            var ret = new ConflictClauseNode(context);
            if (context.ROLLBACK() != null)
            {
                ret.Choice = ConflictChoice.Rollback;
            }
            else if (context.ABORT() != null)
            {
                ret.Choice = ConflictChoice.Abort;
            }

            else if (context.FAIL() != null)
            {
                ret.Choice = ConflictChoice.Fail;
            }
            else if (context.IGNORE() != null)
            {
                ret.Choice = ConflictChoice.Ignore;
            }
            else if (context.REPLACE() != null)
            {
                ret.Choice = ConflictChoice.Replace;
            }

            return ret;
        }

        public override SQLiteParseTreeNode VisitTable_constraint__index_clause(SQLiteParserSimpleParser.Table_constraint__index_clauseContext context)
        {
            var ret = new IndexClauseNode(context);
            if (context.PRIMARY() != null)
            {
                ret.IndexUniqueness = PrimaryKeyOrUnique.PrimaryKey;
            }
            else if (context.UNIQUE() != null)
            {
                ret.IndexUniqueness = PrimaryKeyOrUnique.Unique;
            }

            ret.IndexColumns =
                context.table_constraint__indexed_columns()
                    .table_constraint__indexed_column()
                    .Select(i => i.Accept(this))
                    .Cast<IndexedColumnNode>()
                    .ToList();
            ret.ConflictClauseNode = context.conflict_clause().Accept(this) as ConflictClauseNode;

            return ret;
        }

        public override SQLiteParseTreeNode VisitTable_constraint__indexed_column(SQLiteParserSimpleParser.Table_constraint__indexed_columnContext context)
        {
            return CreateIndexedColumnNode(context);
        }

        public override SQLiteParseTreeNode VisitTable_constraint__foreign_key_constraint(SQLiteParserSimpleParser.Table_constraint__foreign_key_constraintContext context)
        {
            var ret = new TableConstraintForeignKeyNode(context)
            {
                FieldNames = context.table_constraint__parens_field_list()
                    .foreign_key_clause__column_list()
                    .ID()
                    .Select(i => i.GetText())
                    .ToList(),
                ForeignKeyClauseNode = context.foreign_key_clause().Accept(this) as ForeignKeyClauseNode
            };

            return ret;
        }

        



        public override SQLiteParseTreeNode VisitColumn_constraint(SQLiteParserSimpleParser.Column_constraintContext context)
        {
            ColumnConstraintNode ret = null;

            ret = context.column_constraint__postfix().Accept(this) as ColumnConstraintNode;


            if (context.ID() != null)
                    ret.Name = context.ID().GetText();


            return ret;
        }

        public override SQLiteParseTreeNode VisitColumn_constraint__postfix(SQLiteParserSimpleParser.Column_constraint__postfixContext context)
        {
            if (context.NOT() != null)
            {
                return new NotNullConstraintNode(context)
                {
                    ConflictClause = context.conflict_clause().Accept(this) as ConflictClauseNode
                };
            }
            else if (context.PRIMARY() != null)
            {
                var ret = new PrimaryKeyConstraintNode(context);
                if (context.ASC() != null)
                {
                    ret.Order = SortOrder.Asc;
                }
                else if (context.DESC() != null)
                {
                    ret.Order = SortOrder.Desc;
                }

                ret.ConflictClause = context.conflict_clause().Accept(this) as ConflictClauseNode;

                if (context.AUTOINCREMENT() != null)
                {
                    ret.AutoIncrement = true;
                }

                return ret;
            }

            else if (context.UNIQUE() != null)
            {
                return new UniqueConstraintNode(context)
                {
                    ConflictClause = context.conflict_clause().Accept(this) as ConflictClauseNode
                };
            }

            else if (context.CHECK_C() != null)
            {
                return new CheckConstraintNode(context) {Expr = context.expr().GetText()};
            }

            else if (context.DEFAULT() != null )
            {
                var ret = new DefaultConstraintNode(context);
                if (context.signed_number() != null)
                {
                    ret.Value = context.signed_number().GetText();
                }
                else if (context.literal_value() != null)
                {
                    ret.Value = context.literal_value().GetText();
                }

                else if (context.expr() != null)
                {
                    ret.Value = "( " + context.expr().GetText() + " )";
                }
                

                return ret;
            }
            else if (context.NULL() != null)
            {
                //it's a default null but sqlite allows it and says it's null
                var ret = new DefaultConstraintNode(context) {Value = "NULL"};
                return ret;
            }

            else if (context.COLLATE() != null)
            {
                return new CollateConstraintNode(context) {CollationName = context.collation_name().GetText()};
            }
            else if (context.foreign_key_clause() != null)
            {
                return context.foreign_key_clause().Accept(this) as ColumnConstraintNode;
            }

            throw new Exception("we should never get here.");
        }

        public override SQLiteParseTreeNode VisitForeign_key_clause(SQLiteParserSimpleParser.Foreign_key_clauseContext context)
        {
            var ret =  new ForeignKeyClauseNode(context) {TableName = context.table_name().GetText()};

            if (context.foreign_key_clause__parens_field_list() != null)
            {
                ret.FieldList = context.foreign_key_clause__parens_field_list().foreign_key_clause__column_list().ID().Select(i => i.GetText()).ToList();
            }

            if (!context.foreign_key_clause__on_delete().IsNullOrEmpty())
            {
                ret.ForeignOnDelete =  context.foreign_key_clause__on_delete().Select(i => i.Accept(this) as ForeignOnDeleteNode).ToList();
            }

            if (!context.foreign_key_clause__match().IsNullOrEmpty())
            {
                ret.ForeignMatch = context.foreign_key_clause__match().Select(i => i.Accept(this) as ForeignMatchNode).ToList();
            }

            if (!context.foreign_key_clause__on_update().IsNullOrEmpty())
            {
                ret.ForeignOnUpdate =
                    context.foreign_key_clause__on_update().Select(i => i.Accept(this) as ForeignOnUpdateNode).ToList();
            }

            if (context.foreign_key_clause__deferrable() != null)
            {
                ret.ForeignDeferrable = context.foreign_key_clause__deferrable().Accept(this) as ForeignDeferrableNode;
            }

            return ret;
        }

        public override SQLiteParseTreeNode VisitForeign_key_clause__on_delete(SQLiteParserSimpleParser.Foreign_key_clause__on_deleteContext context)
        {
            var ret = new ForeignOnDeleteNode(context) {Action = ParseCommonDeleteUpdateAction(context)};
            return ret;

        }

        public override SQLiteParseTreeNode VisitForeign_key_clause__on_update(SQLiteParserSimpleParser.Foreign_key_clause__on_updateContext context)
        {
            var ret = new ForeignOnUpdateNode(context) {Action = ParseCommonDeleteUpdateAction(context)};
            return ret;
        }

        public override SQLiteParseTreeNode VisitForeign_key_clause__deferrable(SQLiteParserSimpleParser.Foreign_key_clause__deferrableContext context)
        {
            var ret = new ForeignDeferrableNode(context);
            if (context.NOT() != null)
                ret.IsDeferrable = true;

            if (context.IMMEDIATE() != null)
                ret.InitiallyImmediate = true;
            else if (context.DEFERRED() != null)
                ret.InitiallyImmediate = false;

            return ret;
        }

        public override SQLiteParseTreeNode VisitForeign_key_clause__match(SQLiteParserSimpleParser.Foreign_key_clause__matchContext context)
        {
            var ret = new ForeignMatchNode(context) {Id = context.ID().GetText()};
            return ret;
        }

        private IndexedColumnNode CreateIndexedColumnNode(dynamic context)
        {
            var ret = new IndexedColumnNode(context) { Id = context.ID(0).GetText() };
            if (context.COLLATE() != null)
            {
                ret.CollationId = context.ID(1).GetText();
            }

            if (context.ASC() != null)
            {
                ret.Order = SortOrder.Asc;
            }
            else if (context.DESC() != null)
            {
                ret.Order = SortOrder.Desc;
            }

            return ret;
        }

        private static ForeignDeleteOrUpdateAction ParseCommonDeleteUpdateAction(dynamic context)
        {
            if (context.NULL() != null)
            {
                return ForeignDeleteOrUpdateAction.SetNull;
            }
            else if (context.DEFAULT() != null)
            {
                return ForeignDeleteOrUpdateAction.SetDefault;
            }

            else if (context.CASCADE() != null)
            {
               return  ForeignDeleteOrUpdateAction.Cascade;
            }
            else if (context.RESTRICT() != null)
            {
                return ForeignDeleteOrUpdateAction.Restrict;
            }
            else if (context.NO() != null)
            {
                return ForeignDeleteOrUpdateAction.NoAction;
            }

            throw new Exception("we shouldn't ever get here");
        }
        
    }
}
