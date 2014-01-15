using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Outercurve.SQLiteCreateTree.AlterTable.Action;
using Outercurve.SQLiteCreateTree.Nodes;
using Outercurve.SQLiteCreateTree.Nodes.ColumnConstraint;
using Outercurve.SQLiteParser;

namespace Outercurve.SQLiteCreateTree.AlterTable
{
    public class AlterTableAdapter
    {
        public AlterTableAdapter(CreateTableNode node, IEnumerable<CreateIndexNode> indexNodes)
        {
            CreateTableNode = node;
            CreateIndexNodes = indexNodes.ToList();
        }
        public CreateTableNode CreateTableNode { get; private set; }
        public IList<CreateIndexNode> CreateIndexNodes { get; private set; }

        public string[] AlterTableStatements(AlterTableCommand command)
        {
            
        }


        internal static TypeNameNode CreateTypeNameNode(string type)
        {
            
            return SQLiteParseVisitor.ParseString<TypeNameNode>(type, i => i.type_name());
        }

        internal static ColumnDefNode CreateColumnNode(CreateColumnCommand command)
        {
            var ret = new ColumnDefNode { ColumnName = command.ColumnName, ColumnConstraints = new List<ColumnConstraintNode>() };

            //dialect converts DbType.Int16-64 to "INT" not "INTEGER" and only INTEGER columns can be autoincremented. This fixes that.
            string correctType = command.IsIdentity ? "INTEGER" : command.DbType;

            ret.TypeNameNode = CreateTypeNameNode(correctType);

            //not quite right but should work

            if (command.IsIdentity || command.IsPrimaryKey)
            {
                var primKey = new PrimaryKeyConstraintNode();
                if (command.IsIdentity)
                {
                    primKey.AutoIncrement = true;
                }
                ret.ColumnConstraints.Add(primKey);
            }

            if (command.Default != null)
            {
                ret.ColumnConstraints.Add(new DefaultConstraintNode { Value = ConvertToSqlValue(command.Default) });
            }

            if (command.IsNotNull)
            {
                ret.ColumnConstraints.Add(new NotNullConstraintNode());
            }
            else if (command.Default == null && !command.IsPrimaryKey && !command.IsUnique)
            {
                ret.ColumnConstraints.Add(new DefaultConstraintNode { Value = "NULL" });
            }

            if (command.IsUnique)
            {
                ret.ColumnConstraints.Add(new UniqueConstraintNode());
            }

            return ret;
        }

        internal static string ConvertToSqlValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            TypeCode typeCode = Type.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.String:
                case TypeCode.Char:
                    return String.Concat("'", Convert.ToString(value).Replace("'", "''"), "'");
                case TypeCode.Boolean:
                    return (bool)value ? "1" : "0";
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return Convert.ToString(value, CultureInfo.InvariantCulture);
                case TypeCode.DateTime:
                    return String.Concat("'", Convert.ToString(value, CultureInfo.InvariantCulture), "'");
            }

            return "null";
        }
    }
    
    
}
