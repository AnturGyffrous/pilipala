using System.Linq;

namespace Pilipala.Data.Extensions
{
    internal static class LinqExtension
    {
        internal static TSource[] Take<TSource>(this TSource[] source, int count, TSource terminator)
        {
            for (var i = 0; i < count && i < source.Length; i++)
            {
                if (source[i].Equals(terminator))
                {
                    return source.Take(i).ToArray();
                }
            }

            return source.Take(count).ToArray();
        }
    }
}
