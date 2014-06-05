using System;

using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase.Fields
{
    internal class DateField : Field
    {
        private DateTime _value;

        public DateField(byte[] buffer)
            : base(buffer)
        {
            if (Length != 8 || DecimalCount != 0)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            Type = typeof(DateTime);
            TypeName = "Date";
        }

        public override object Value
        {
            get
            {
                return _value;
            }
        }

        public override void Parse(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}
