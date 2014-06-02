using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Pilipala.Data.DBase;

namespace Pilipala.Data.UnitTests.DBase
{
    [TestFixture]
    public class FieldTests
    {
        private byte[] GetFieldData(string name, char type, byte length, byte decimalCount, byte workAreaId, bool productionMdx)
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
        public void CanParseCharacterField()
        {
            var data = GetFieldData("Char Field", 'C', 50, 3, 1, true);
            var field = Field.Parse(data);

            Assert.That(field.Name, Is.EqualTo("Char Field"));
            Assert.That(field.Type, Is.EqualTo("Character"));
            Assert.That(field.Length, Is.EqualTo(50));
            Assert.That(field.DecimalCount, Is.EqualTo(3));
            Assert.That(field.WorkAreaID, Is.EqualTo(1));
            Assert.That(field.ProductionMdx, Is.True);
        }

        [Test]
        public void WillGetAnExceptionIfNotEnoughBytesHaveBeenProvided()
        {
            Assert.Throws<InvalidOperationException>(() => Field.Parse(new byte[] { 0, 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void WillGetAnExceptionIfTheFieldIsAnUnknownType()
        {
            var data = GetFieldData("Unknown Type", 'U', 10, 0, 0, false);
            Assert.Throws<InvalidOperationException>(() => Field.Parse(data));
        }
    }
}
