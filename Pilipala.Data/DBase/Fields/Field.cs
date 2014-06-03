using System;
using System.Text;

using Pilipala.Data.Extensions;
using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase.Fields
{
    internal abstract class Field
    {
        protected Field(byte[] buffer)
        {
            var name = buffer.Take<byte>(11, 0);
            Name = Encoding.ASCII.GetString(name, 0, name.Length);
            Length = buffer[16];
            DecimalCount = buffer[17];
            WorkAreaID = buffer[20];
            ProductionMdx = buffer[31] != 0;

            if (Length == 0)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }
        }

        public int DecimalCount { get; private set; }

        public int Length { get; private set; }

        public string Name { get; private set; }

        public bool ProductionMdx { get; private set; }

        public string Type { get; protected set; }

        public int WorkAreaID { get; private set; }

        internal static Field Parse(byte[] buffer)
        {
            if (buffer.Length != 32)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            switch ((char)buffer[11])
            {
                case 'C':
                    return new CharacterField(buffer);
                case 'D':
                    return new DateField(buffer);
                case 'N':
                    return new NumericField(buffer);
                case 'F':
                    return new FloatField(buffer);
                case 'L':
                    return new LogicalField(buffer);
                default:
                    throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }
        }
    }
}
