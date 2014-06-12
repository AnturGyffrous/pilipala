using System;

using NUnit.Framework;

using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.UnitTests.DBase.Fields
{
    [TestFixture]
    public class LogicalFieldTests
    {
        [Test]
        public void CanParseLogicalField()
        {
            var data = FieldTests.GetFieldData("Bool Field", 'L', 1);

            var field = Field.ParseMetaData(data);
            Assert.That(field.Name, Is.EqualTo("Bool Field"));
            Assert.That(field.Type, Is.EqualTo(typeof(bool)));
            Assert.That(field.TypeName, Is.EqualTo("Logical"));
            Assert.That(field.Length, Is.EqualTo(1));
            Assert.That(field.DecimalCount, Is.EqualTo(0));
        }

        [Test]
        public void WillGetAnExceptionIfLogicalFieldDoesNotHaveLengthOfOne()
        {
            var data = FieldTests.GetFieldData("Bool Field", 'L', 2);
            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(data));
        }

        [Test]
        public void WillGetAnExceptionIfLogicalFieldHasDecimalCountGreaterThanZero()
        {
            var data = FieldTests.GetFieldData("Bool Field", 'L', 1, 3);
            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(data));
        }
    }
}
