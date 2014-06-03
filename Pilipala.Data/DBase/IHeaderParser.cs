using System.Collections.Generic;

using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.DBase
{
    internal interface IHeaderParser
    {
        IEnumerable<Field> Fields { get; }

        int RecordCount { get; }
    }
}
