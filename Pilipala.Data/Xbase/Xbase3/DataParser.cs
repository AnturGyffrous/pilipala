using System;
using System.Collections.Generic;
using System.IO;

using JetBrains.Annotations;

namespace Pilipala.Data.Xbase.Xbase3
{
    public class DataParser : IXbaseDataParser
    {
        private DataParser(Stream stream)
        {
            var data = new byte[32];
            if (stream.Read(data, 0, 32) != 32)
            {
                throw new InvalidOperationException(
                    "Not enough data could be loaded from the stream to read the header. An Xbase data file must contain at least 32 bytes of information.");
            }

            CheckVersion(data[0]);

            LastUpdated = GetLastUpdatedDate(data[1], data[2], data[3]);

            RecordsAffected = ValidateRecordCount(BitConverter.ToInt32(data, 4));

            var headerLength = ValidateHeaderLength(BitConverter.ToInt16(data, 8));

            RecordLength = ValidateRecordLength(BitConverter.ToInt16(data, 10));

            IncompleteTransaction = data[14] != 0;

            Encrypted = data[15] != 0;

            Mdx = data[28] != 0;

            LanguageDriverID = data[29];

            var fieldCount = (headerLength - 33) / 32;
            var fields = new List<IField>(fieldCount);
            data = new byte[32];
            stream.Read(data, 0, 32);
            fields.Add(Field.Create(data));
            Fields = fields;
        }

        public bool Encrypted { get; private set; }

        public IEnumerable<IField> Fields { get; private set; }

        public bool IncompleteTransaction { get; private set; }

        public int LanguageDriverID { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public bool Mdx { get; private set; }

        public int RecordLength { get; private set; }

        public int RecordsAffected { get; private set; }

        public static DataParser Create(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            return new DataParser(stream);
        }

        [AssertionMethod]
        private static void CheckVersion(byte version)
        {
            if (version != 3)
            {
                throw new InvalidOperationException("Invalid or unknown version.");
            }
        }

        private static DateTime GetLastUpdatedDate(byte year, byte month, byte day)
        {
            try
            {
                return new DateTime(year + 1900, month, day);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Unable to parse the date of last update.", exception);
            }
        }

        private static short ValidateHeaderLength(short headerLength)
        {
            if (headerLength % 32 != 1)
            {
                throw new InvalidOperationException("The reported header length is invalid.");
            }

            return headerLength;
        }

        private static int ValidateRecordCount(int recordCount)
        {
            if (recordCount < 0)
            {
                throw new InvalidOperationException("Unable to parse the record count reported in the header, the number was negative.");
            }

            return recordCount;
        }

        private static short ValidateRecordLength(short recordLength)
        {
            if (recordLength <= 1)
            {
                throw new InvalidOperationException("The sum of the lengths of all the fields could not be parsed, it must have a value of at least two.");
            }

            return recordLength;
        }
    }
}
