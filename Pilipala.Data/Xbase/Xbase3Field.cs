using System;
using System.Linq;
using System.Text;

namespace Pilipala.Data.Xbase
{
    internal abstract class Xbase3Field : IField
    {
        protected Xbase3Field(byte[] buffer)
        {
            var name = Take<byte>(buffer, 11, 0);
            Name = Encoding.ASCII.GetString(name, 0, name.Length);
            Length = buffer[16];
        }

        public int Length { get; private set; }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public string TypeName { get; private set; }

        public object Value { get; private set; }

        public void Parse(byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }

        internal static Xbase3Field Create(byte[] buffer)
        {
            switch ((char)buffer[11])
            {
                case 'N':
                    return new Xbase3NumericField(buffer);
            }

            throw new NotImplementedException();
        }

        private static TSource[] Take<TSource>(TSource[] source, int count, TSource terminator)
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
