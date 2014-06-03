using System.Collections.Generic;

using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.DBase
{
    internal interface IMetaData
    {
        IEnumerable<Field> Fields { get; }

        int RecordsAffected { get; }
    }
}
