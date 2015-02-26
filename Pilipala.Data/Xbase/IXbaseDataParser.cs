using System.Threading.Tasks;

namespace Pilipala.Data.Xbase
{
    public interface IXbaseDataParser
    {
        void Close();

        bool Read();

        Task<bool> ReadAsync();
    }
}
