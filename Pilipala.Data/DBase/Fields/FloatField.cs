using System;

using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase.Fields
{
    internal class FloatField : Field
    {
        public FloatField(byte[] buffer)
            : base(buffer)
        {
            if (Length > 20)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            Type = typeof(double);
            TypeName = "Float";
        }
    }
}
