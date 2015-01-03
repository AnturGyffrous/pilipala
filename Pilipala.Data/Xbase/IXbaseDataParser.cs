using System.Collections.Generic;

namespace Pilipala.Data.Xbase
{
    public interface IXbaseDataParser
    {
        IEnumerable<IField> Fields { get; }
    }
}
