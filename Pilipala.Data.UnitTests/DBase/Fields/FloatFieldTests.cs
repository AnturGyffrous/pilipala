using System;
using System.Text;

using NUnit.Framework;

using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.UnitTests.DBase.Fields
{
    [TestFixture]
    public class FloatFieldTests
    {
        [Test]
        public void CanGetValueFromFloatField()
        {
            const double fieldValue = 598724.6171;
            var data = FieldTests.GetFieldData("Float Fld", 'F', 13, 4);
            var field = Field.ParseMetaData(data);
            field.Parse(Encoding.ASCII.GetBytes(string.Format("{0,13}", fieldValue)));
            Assert.That(field.Value, Is.EqualTo(fieldValue));
        }

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
