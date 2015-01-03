using System;
using System.Text;

using Pilipala.Data.Extensions;

namespace Pilipala.Data.Xbase.Xbase3
{
    internal abstract class Field : IField
    {
        protected Field(byte[] buffer)
        {
            var name = buffer.Take<byte>(11, 0);
            Name = Encoding.ASCII.GetString(name, 0, name.Length);
            Length = buffer[16];
        }

        public int Length { get; private set; }

        public string Name { get; private set; }

        public Type Type { get; protected set; }

        public string TypeName { get; protected set; }

        public object Value { get; private set; }

        public void Parse(byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }

        internal static Field Create(byte[] buffer)
        {
            switch ((char)buffer[11])
            {
                case 'N':
                    return new NumericField(buffer);
            }

            throw new NotImplementedException();
        }
    }
}
