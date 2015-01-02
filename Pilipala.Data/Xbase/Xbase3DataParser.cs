using System;
using System.IO;

using JetBrains.Annotations;

namespace Pilipala.Data.Xbase
{
    public class Xbase3DataParser : IXbaseDataParser
    {
        private Xbase3DataParser(Stream stream)
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

            ValidateHeaderLength(BitConverter.ToInt16(data, 8));

            RecordLength = BitConverter.ToInt16(data, 10);

            IncompleteTransaction = data[14] != 0;

            Encrypted = data[15] != 0;

            Mdx = data[28] != 0;

            LanguageDriverID = data[29];
        }

        public bool Encrypted { get; private set; }

        public bool IncompleteTransaction { get; private set; }

        public int LanguageDriverID { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public bool Mdx { get; private set; }

        public int RecordLength { get; private set; }

        public int RecordsAffected { get; private set; }

        public static Xbase3DataParser Create(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            return new Xbase3DataParser(stream);
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

        [AssertionMethod]
        private static void ValidateHeaderLength(short headerLength)
        {
            if (headerLength % 32 != 1)
            {
                throw new InvalidOperationException("The reported header length is invalid.");
            }
        }

        private static int ValidateRecordCount(int recordCount)
        {
            if (recordCount < 0)
            {
                throw new InvalidOperationException("Unable to parse the record count reported in the header, the number was negative.");
            }

            return recordCount;
        }
    }
}
