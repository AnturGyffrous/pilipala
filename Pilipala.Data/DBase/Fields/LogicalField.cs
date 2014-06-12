using System;

using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase.Fields
{
    internal class LogicalField : Field
    {
        private bool? _value;

        public LogicalField(byte[] buffer)
            : base(buffer)
        {
            if (Length != 1 || DecimalCount != 0)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            Type = typeof(bool);
            TypeName = "Logical";
        }

        public override object Value
        {
            get
            {
                return _value;
            }
        }

        protected override void ParseData(byte[] buffer)
        {
            switch ((char)buffer[0])
            {
                case 'Y':
                case 'y':
                case 'T':
                case 't':
                    _value = true;
                    return;
                case 'N':
                case 'n':
                case 'F':
                case 'f':
                    _value = false;
                    return;
                default:
                    _value = null;
                    return;
            }
        }
    }
}
