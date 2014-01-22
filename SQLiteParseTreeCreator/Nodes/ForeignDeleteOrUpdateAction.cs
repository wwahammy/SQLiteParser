// Copyright (c) 2014, The Outercurve Foundation. The software is licensed under the (the "License"); you may not use the software except in compliance with the License.
namespace Outercurve.SQLiteCreateTree.Nodes
{
    public enum ForeignDeleteOrUpdateAction
    {
        SetNull,
        SetDefault,
        Cascade,
        Restrict,
        NoAction
    }
}