using System;

namespace Pilipala.Data.Xbase.Xbase3
{
    internal class FloatField : Field
    {
        internal FloatField(byte[] buffer)
            : base(buffer)
        {
            if (Length != 20)
            {
                throw new InvalidOperationException("Unable to parse the field descriptors, a float field must have a length of 20.");
            }

            Type = typeof(double);
            TypeName = "Float";
        }
    }
}
