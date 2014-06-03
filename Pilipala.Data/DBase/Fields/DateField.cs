using System;

using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase.Fields
{
    internal class DateField : Field
    {
        public DateField(byte[] buffer)
            : base(buffer)
        {
            if (Length != 8 || DecimalCount != 0)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            Type = "Date";
        }
    }
}
