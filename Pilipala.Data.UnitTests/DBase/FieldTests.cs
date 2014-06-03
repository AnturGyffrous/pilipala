using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.UnitTests.DBase
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
        public void CanParseCharacterField()
        {
            var data = GetFieldData("Char Field", 'C', 50, 3, 1, true);
            var field = Field.Parse(data);

            Assert.That(field.Name, Is.EqualTo("Char Field"));
            Assert.That(field.Type, Is.EqualTo(typeof(string)));
            Assert.That(field.TypeName, Is.EqualTo("Character"));
            Assert.That(field.Length, Is.EqualTo(50));
            Assert.That(field.DecimalCount, Is.EqualTo(3));
            Assert.That(field.WorkAreaID, Is.EqualTo(1));
            Assert.That(field.ProductionMdx, Is.True);
        }

        [Test]
        public void CanParseDateField()
        {
            var data = GetFieldData("Date Field", 'D', 8);

            var field = Field.Parse(data);
            Assert.That(field.Name, Is.EqualTo("Date Field"));
            Assert.That(field.Type, Is.EqualTo(typeof(DateTime)));
            Assert.That(field.TypeName, Is.EqualTo("Date"));
            Assert.That(field.Length, Is.EqualTo(8));
            Assert.That(field.DecimalCount, Is.EqualTo(0));
        }

        [Test]
        public void CanParseFloatField()
        {
            var data = GetFieldData("Float Fld", 'F', 20, 8);

            var field = Field.Parse(data);
            Assert.That(field.Name, Is.EqualTo("Float Fld"));
            Assert.That(field.Type, Is.EqualTo(typeof(double)));
            Assert.That(field.TypeName, Is.EqualTo("Float"));
            Assert.That(field.Length, Is.EqualTo(20));
            Assert.That(field.DecimalCount, Is.EqualTo(8));
        }

        [Test]
        public void CanParseLogicalField()
        {
            var data = GetFieldData("Bool Field", 'L', 1);

            var field = Field.Parse(data);
            Assert.That(field.Name, Is.EqualTo("Bool Field"));
            Assert.That(field.Type, Is.EqualTo(typeof(bool)));
            Assert.That(field.TypeName, Is.EqualTo("Logical"));
            Assert.That(field.Length, Is.EqualTo(1));
            Assert.That(field.DecimalCount, Is.EqualTo(0));
        }

        [Test]
        public void CanParseNumericField()
        {
            var data = GetFieldData("Num Field", 'N', 10, 2);

            var field = Field.Parse(data);
            Assert.That(field.Name, Is.EqualTo("Num Field"));
            Assert.That(field.Type, Is.EqualTo(typeof(double)));
            Assert.That(field.TypeName, Is.EqualTo("Numeric"));
            Assert.That(field.Length, Is.EqualTo(10));
            Assert.That(field.DecimalCount, Is.EqualTo(2));
        }

        [Test]
        public void WillGetAnExceptionIfDateFieldDoesNotHaveDecimalLengthOfZero()
        {
            var data = GetFieldData("Date Field", 'D', 8, 4);
            Assert.Throws<InvalidOperationException>(() => Field.Parse(data));
        }

        [Test]
        public void WillGetAnExceptionIfDateFieldDoesNotHaveLengthOfEight()
        {
            var data = GetFieldData("Date Field", 'D', 10);
            Assert.Throws<InvalidOperationException>(() => Field.Parse(data));
        }

        [Test]
        public void WillGetAnExceptionIfDecimalCountIsGreaterThanFieldLength()
        {
            var data = GetFieldData("Float Fld", 'F', 10, 15);
            Assert.Throws<InvalidOperationException>(() => Field.Parse(data));
        }

        [Test]
        public void WillGetAnExceptionIfFloatFieldIsLongerThanTwenty()
        {
            var data = GetFieldData("Float Fld", 'F', 21, 4);
            Assert.Throws<InvalidOperationException>(() => Field.Parse(data));
        }

        [Test]
        public void WillGetAnExceptionIfLengthIsZero()
        {
            var data = GetFieldData("Char Field", 'C', 0);

            Assert.Throws<InvalidOperationException>(() => Field.Parse(data));
        }

        [Test]
        public void WillGetAnExceptionIfLogicalFieldDoesNotHaveLengthOfOne()
        {
            var data = GetFieldData("Bool Field", 'L', 2);
            Assert.Throws<InvalidOperationException>(() => Field.Parse(data));
        }

        [Test]
        public void WillGetAnExceptionIfLogicalFieldHasDecimalCountGreaterThanZero()
        {
            var data = GetFieldData("Bool Field", 'L', 1, 3);
            Assert.Throws<InvalidOperationException>(() => Field.Parse(data));
        }

        [Test]
        public void WillGetAnExceptionIfNotEnoughBytesHaveBeenProvided()
        {
            Assert.Throws<InvalidOperationException>(() => Field.Parse(new byte[] { 0, 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void WillGetAnExceptionIfNumericFieldIsLongerThanTwenty()
        {
            var data = GetFieldData("Num Field", 'N', 21, 4);
            Assert.Throws<InvalidOperationException>(() => Field.Parse(data));
        }

        [Test]
        public void WillGetAnExceptionIfTheFieldIsAnUnknownType()
        {
            var data = GetFieldData("Unknown TypeName", 'U', 10);
            Assert.Throws<InvalidOperationException>(() => Field.Parse(data));
        }
    }
}
