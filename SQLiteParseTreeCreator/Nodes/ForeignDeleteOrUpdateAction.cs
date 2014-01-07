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