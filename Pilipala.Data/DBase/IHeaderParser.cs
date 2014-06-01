namespace Pilipala.Data.DBase
{
    internal interface IHeaderParser
    {
        int FieldCount { get; }

        int RecordCount { get; }
    }
}
