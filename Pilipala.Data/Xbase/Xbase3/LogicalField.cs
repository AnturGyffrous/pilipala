using System;

namespace Pilipala.Data.Xbase.Xbase3
{
    internal class LogicalField : Field
    {
        internal LogicalField(byte[] buffer)
            : base(buffer)
        {
            if (Length > 1)
            {
                throw new InvalidOperationException("Unable to parse the field descriptors, a logical field cannot have a length greater than 1.");
            }

            Type = typeof(bool);
            TypeName = "Logical";
        }
    }
}
