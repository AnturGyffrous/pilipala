using System;
using System.Text;

using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase.Fields
{
    internal class CharacterField : Field
    {
        private string _value;

        public CharacterField(byte[] buffer)
            : base(buffer)
        {
            if (DecimalCount != 0)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            Type = typeof(string);
            TypeName = "Character";
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
            _value = Encoding.ASCII.GetString(buffer).Trim();
        }
    }
}
