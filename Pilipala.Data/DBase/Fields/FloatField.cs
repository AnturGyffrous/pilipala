﻿using System;
using System.Text;

using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase.Fields
{
    internal class FloatField : Field
    {
        private double? _value;

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

        public override object Value
        {
            get
            {
                return _value;
            }
        }

        protected override void ParseData(byte[] buffer, int offset)
        {
            double value;
            if (double.TryParse(Encoding.ASCII.GetString(buffer, offset, Length).Trim(), out value))
            {
                _value = value;
            }
            else
            {
                _value = null;
            }
        }
    }
}
