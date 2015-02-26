using System.Threading.Tasks;

namespace Pilipala.Tests.Extensions
{
    public static class AsyncExtensions
    {
        public static void IgnoreAwaitWarning(this Task task)
        {
        }
    }
}
