using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pilipala.Data.UnitTests.Xbase.Xbase3
{
    internal class DataGenerator
    {
        public DataGenerator()
        {
            Version = 3;
            LastUpdatedYear = 115;
            LastUpdatedMonth = 10;
            LastUpdatedDay = 21;
            RecordCount = 0;
            HeaderByteCount = 65;
            RecordLength = 2;
            IncompleteTransactionFlag = 0;
            EncryptionFlag = 0;
            MdxFlag = 0;
            LanguageDriverId = 0;
            Fields = new List<byte[]> { CreateFieldDescriptor("ID", 'N', 10) };
        }

        internal byte EncryptionFlag { get; set; }

        internal IEnumerable<byte[]> Fields { get; set; }

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
                           0, 0 // Reserved
                       };

            var array = BitConverter.GetBytes(RecordCount);
            Array.Copy(array, 0, data, 4, array.Length);

            array = BitConverter.GetBytes(HeaderByteCount);
            Array.Copy(array, 0, data, 8, array.Length);

            array = BitConverter.GetBytes(RecordLength);
            Array.Copy(array, 0, data, 10, array.Length);

            var headerTerminator = new byte[] { 0xd };

            return data.Concat(Fields.SelectMany(x => x)).Concat(headerTerminator).ToArray();
        }

        private static byte[] CreateFieldDescriptor(string name, char type, byte length, byte decimalCount = 0, byte workAreaId = 0, bool productionMdx = false)
        {
            // -----------------------------------------------------------------------------------------------------------------------
            // | Byte  | Contents | Meaning                                                                                          |
            // |-------|----------|--------------------------------------------------------------------------------------------------|
            // | 0-10  | 11 bytes | Field name in ASCII (terminated by 00h).                                                         |
            // | 11    | 1 byte   | Field type in ASCII (C, D, F, L, M, or N).                                                       |
            // | 12-15 | 4 bytes  | Field data address (in memory !!! dBASE III+).                                                   |
            // | 16    | 1 byte   | Field length in binary.                                                                          |
            // | 17    | 1 byte   | Field decimal count in binary.                                                                   |
            // | 18-19 | 2 bytes  | Reserved for multi-user dBASE.                                                                   |
            // | 20    | 1 byte   | Work area ID.                                                                                    |
            // | 21-22 | 2 bytes  | Reserved for multi-user dBASE.                                                                   |
            // | 23    | 1 byte   | Flag for SET FIELDS.                                                                             |
            // | 24-30 | 7 bytes  | Reserved.                                                                                        |
            // | 31    | 1 byte   | Production MDX field flag; 01H if field has an index tag in the production MDX file, 00H if not. |
            // -----------------------------------------------------------------------------------------------------------------------
            var data = Encoding
                .ASCII
                .GetBytes(name)
                .Concat(Enumerable.Repeat((byte)0, 11))
                .Take(11)
                .Concat(Enumerable.Repeat((byte)0, 21))
                .ToArray();

            data[11] = (byte)type;
            data[16] = length;
            data[17] = decimalCount;
            data[20] = workAreaId;
            data[31] = productionMdx
                           ? (byte)1
                           : (byte)0;

            return data;
        }
    }
}
