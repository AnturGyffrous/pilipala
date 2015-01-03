using System;

namespace Pilipala.Data.Xbase.Xbase3
{
    internal class DateField : Field
    {
        internal DateField(byte[] buffer)
            : base(buffer)
        {
            if (Length != 8)
            {
                throw new InvalidOperationException("Unable to parse the field descriptors, a date field must have a length of 8.");
            }

            Type = typeof(DateTime);
            TypeName = "Date";
        }
    }
}
