using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pilipala.Data.Xbase
{
    public interface IXbaseDataParser
    {
        IEnumerable<IField> Fields { get; }

        int RecordsAffected { get; }

        void Close();

        bool Read();

        Task<bool> ReadAsync();
    }
}
