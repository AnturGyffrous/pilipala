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
            switch ((char)buffer[11])
            {
                case 'C':
                    Type = "Character";
                    break;
                default:
                    throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            Length = buffer[16];
            DecimalCount = buffer[17];
            WorkAreaID = buffer[20];
            ProductionMdx = buffer[31] != 0;
        }

        public int DecimalCount { get; private set; }

        public int Length { get; private set; }

        public string Name { get; private set; }

        public bool ProductionMdx { get; private set; }

        public string Type { get; private set; }

        public int WorkAreaID { get; private set; }

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
