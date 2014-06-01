using System;
using System.Text;

using Pilipala.Data.Extensions;
using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase
{
    internal class Field
    {
        private Field(byte[] buffer)
        {
            var name = buffer.Take<byte>(11, 0);
            Name = Encoding.ASCII.GetString(name, 0, name.Length);
        }

        public string Name { get; private set; }

        internal static Field Parse(byte[] buffer)
        {
            if (buffer.Length != 32)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            return new Field(buffer);
        }
    }
}
