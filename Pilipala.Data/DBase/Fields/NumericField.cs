using System;

using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase.Fields
{
    internal class NumericField : Field
    {
        private double _value;

        public NumericField(byte[] buffer)
            : base(buffer)
        {
            if (Length > 20)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            Type = typeof(double);
            TypeName = "Numeric";
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
            throw new NotImplementedException();
        }
    }
}
