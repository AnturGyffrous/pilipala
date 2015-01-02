using System;

namespace Pilipala.Data.UnitTests.Xbase
{
    internal class Xbase3DataGenerator
    {
        public Xbase3DataGenerator()
        {
            Version = 3;
            LastUpdatedYear = 115;
            LastUpdatedMonth = 10;
            LastUpdatedDay = 21;
            RecordCount = 0;
            HeaderByteCount = 33;
            RecordLength = 0;
            IncompleteTransactionFlag = 0;
            EncryptionFlag = 0;
            MdxFlag = 0;
            LanguageDriverId = 0;
        }

        internal byte EncryptionFlag { get; set; }

        internal short HeaderByteCount { get; set; }

        internal byte IncompleteTransactionFlag { get; set; }

        internal byte LanguageDriverId { get; set; }

        internal byte LastUpdatedDay { get; set; }

        internal byte LastUpdatedMonth { get; set; }

        internal byte LastUpdatedYear { get; set; }

        internal byte MdxFlag { get; set; }

        internal int RecordCount { get; set; }

        internal short RecordLength { get; set; }

        internal byte Version { get; set; }

        public byte[] GetData()
        {
            var year = LastUpdatedYear;
            var month = LastUpdatedMonth;
            var day = LastUpdatedDay;

            var data = new byte[]
                       {
                           Version, // Version number
                           year, month, day, // Last updated date without century YYMMDD, valid year interval is 0x00 - 0xff, add to base year of 1900 for interval 1900 - 2155.
                           0, 0, 0, 0, // Record count (little endian)
                           0, 0, // Header byte count (little endian)
                           0, 0, // Sum of lengths of all fields + 1 for deletion flag (little endian)
                           0, 0, // Reserved
                           IncompleteTransactionFlag, // Incomplete transaction flag
                           EncryptionFlag, // Encryption flag
                           0, 0, 0, 0, // Free record thread (reserved for LAN only)
                           0, 0, 0, 0, 0, 0, 0, 0, // Reserved for multi-user dBASE ( dBASE III+ - )
                           MdxFlag, // MDX flag (dBASE IV)
                           LanguageDriverId, // Language driver ID
                           0, // Reserved
                           0xd // Header terminator
                       };

            var array = BitConverter.GetBytes(RecordCount);
            Array.Copy(array, 0, data, 4, array.Length);

            array = BitConverter.GetBytes(HeaderByteCount);
            Array.Copy(array, 0, data, 8, array.Length);

            array = BitConverter.GetBytes(RecordLength);
            Array.Copy(array, 0, data, 10, array.Length);

            return data;
        }
    }
}
