// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
using System;
using System.Data;

namespace Outercurve.SQLiteCreateTree.AlterTable.Action {
    public static class SchemaUtils {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Enum.TryParse<System.Data.DbType>(System.String,System.Boolean,System.Data.DbType@)")]
        public static DbType ToDbType(Type type) {
            DbType dbType;
            switch ( Type.GetTypeCode(type) ) {
                case TypeCode.String:
                    dbType = DbType.String;
                    break;
                case TypeCode.Int32:
                    dbType = DbType.Int32;
                    break;
                case TypeCode.DateTime:
                    dbType = DbType.DateTime;
                    break;
                case TypeCode.Boolean:
                    dbType = DbType.Boolean;
                    break;
                default:
                    Enum.TryParse(Type.GetTypeCode(type).ToString(), true, out dbType);
                    break;
            }

            return dbType;
        }

    }
}
