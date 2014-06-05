﻿using System;

using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase.Fields
{
    internal class LogicalField : Field
    {
        private bool _value;

        public LogicalField(byte[] buffer)
            : base(buffer)
        {
            if (Length != 1 || DecimalCount != 0)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            Type = typeof(bool);
            TypeName = "Logical";
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
