using System.Threading.Tasks;

namespace Pilipala.Tests.Extensions
{
    public static class NSubstituteExtensions
    {
        // ReSharper disable once UnusedParameter.Global
        public static void IgnoreAwaitWarning(this Task task)
        {
        }

        // ReSharper disable once UnusedParameter.Global
        public static void IgnoreUnusedVariable(this object property)
        {
        }
    }
}
