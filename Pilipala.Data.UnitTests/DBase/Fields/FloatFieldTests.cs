using System;

using NUnit.Framework;

using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.UnitTests.DBase.Fields
{
    [TestFixture]
    public class FloatFieldTests
    {
        [Test]
        public void CanParseFloatField()
        {
            var data = FieldTests.GetFieldData("Float Fld", 'F', 20, 8);

            var field = Field.ParseMetaData(data);
            Assert.That(field.Name, Is.EqualTo("Float Fld"));
            Assert.That(field.Type, Is.EqualTo(typeof(double)));
            Assert.That(field.TypeName, Is.EqualTo("Float"));
            Assert.That(field.Length, Is.EqualTo(20));
            Assert.That(field.DecimalCount, Is.EqualTo(8));
        }

        [Test]
        public void WillGetAnExceptionIfFloatFieldIsLongerThanTwenty()
        {
            var data = FieldTests.GetFieldData("Float Fld", 'F', 21, 4);
            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(data));
        }
    }
}
