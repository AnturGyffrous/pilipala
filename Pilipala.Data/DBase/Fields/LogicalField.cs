using System;

using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase.Fields
{
    internal class LogicalField : Field
    {
        public LogicalField(byte[] buffer)
            : base(buffer)
        {
            if (Length != 1)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            Type = "Logical";
        }
    }
}
