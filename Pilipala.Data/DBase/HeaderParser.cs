using System;
using System.IO;

using Pilipala.Data.Resources;

namespace Pilipala.Data.DBase
{
    internal class HeaderParser : IHeaderParser
    {
        public HeaderParser(Stream stream)
        {
            var data = new byte[31];
            if (stream.Read(data, 0, 31) != 31)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            SetLastUpdatedDate(data[0], data[1], data[2]);
            RecordCount = BitConverter.ToInt32(data, 3);
            var headerLength = BitConverter.ToInt16(data, 7);
            RecordLength = BitConverter.ToInt16(data, 9);
            IncompleteTransaction = data[13] != 0;
            Encrypted = data[14] != 0;
            ProductionMdx = data[28] != 0;
            LanguageDriverID = data[29];

            if (RecordCount < 0 || headerLength < 33 || RecordLength < 1)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }
        }

        public bool Encrypted { get; private set; }

        public int FieldCount { get; private set; }

        public bool IncompleteTransaction { get; private set; }

        public int LanguageDriverID { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public bool ProductionMdx { get; private set; }

        public int RecordCount { get; private set; }

        public int RecordLength { get; private set; }

        private void SetLastUpdatedDate(byte year, byte month, byte day)
        {
            if (year > 99 || month < 1 || month > 12 || day < 1 || day > 31)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            var century = year < 79
                              ? 2000
                              : 1900;

            LastUpdated = new DateTime(century + year, month, day);
        }
    }
}
