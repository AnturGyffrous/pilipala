using System;

namespace Pilipala.Data.Xbase.Xbase3
{
    internal class NumericField : Field
    {
        internal NumericField(byte[] buffer)
            : base(buffer)
        {
            if (Length > 20)
            {
                throw new InvalidOperationException("Unable to parse the field descriptors, a numeric field cannot have a length greater than 20.");
            }

            Type = typeof(double);
            TypeName = "Numeric";
        }
    }
}
