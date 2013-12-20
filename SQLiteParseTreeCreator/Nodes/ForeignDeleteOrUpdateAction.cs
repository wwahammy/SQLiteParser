namespace SQLiteParseTreeCreator.Nodes
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