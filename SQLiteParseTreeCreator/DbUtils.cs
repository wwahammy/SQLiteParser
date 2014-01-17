using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Outercurve.SQLiteCreateTree
{
    internal static class DbUtils
    {

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
