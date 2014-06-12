using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.UnitTests.DBase.Fields
{
    [TestFixture]
    public class FieldTests
    {
        internal static byte[] GetFieldData(string name, char type, byte length, byte decimalCount = 0, byte workAreaId = 0, bool productionMdx = false)
        {
            // -----------------------------------------------------------------------------------------------------------------------
            // | Byte  | Contents | Meaning                                                                                          |
            // |-------|----------|--------------------------------------------------------------------------------------------------|
            // | 0-10  | 11 bytes | Field name in ASCII (zero-filled).                                                               |
            // | 11    | 1 byte   | Field type in ASCII (C, D, F, L, M, or N).                                                       |
            // | 12-15 | 4 bytes  | Reserved.                                                                                        |
            // | 16    | 1 byte   | Field length in binary.                                                                          |
            // | 17    | 1 byte   | Field decimal count in binary.                                                                   |
            // | 18-19 | 2 bytes  | Reserved.                                                                                        |
            // | 20    | 1 byte   | Work area ID.                                                                                    |
            // | 21-30 | 10 bytes | Reserved.                                                                                        |
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

        [Test]
        public void WillGetAnExceptionIfDecimalCountIsGreaterThanFieldLength()
        {
            var data = GetFieldData("Float Fld", 'F', 10, 15);
            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(data));
        }

        [Test]
        public void WillGetAnExceptionIfFieldDataSizeIsDifferentToLength()
        {
            var data = GetFieldData("Char Field", 'C', 50);
            var field = Field.ParseMetaData(data);

            Assert.Throws<InvalidOperationException>(() => field.Parse(Encoding.ASCII.GetBytes("Hello, world")));
        }

        [Test]
        public void WillGetAnExceptionIfLengthIsZero()
        {
            var data = GetFieldData("Char Field", 'C', 0);

            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(data));
        }

        [Test]
        public void WillGetAnExceptionIfNotEnoughBytesHaveBeenProvided()
        {
            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(new byte[] { 0, 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void WillGetAnExceptionIfTheFieldIsAnUnknownType()
        {
            var data = GetFieldData("Unknown TypeName", 'U', 10);
            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(data));
        }
    }
}
