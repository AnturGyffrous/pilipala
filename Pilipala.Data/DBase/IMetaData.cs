using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.DBase
{
    internal interface IMetaData
    {
        IField[] Fields { get; }

        int RecordsAffected { get; }

        bool Read();
    }
}
