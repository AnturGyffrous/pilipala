using System;
using System.Text;

using NUnit.Framework;

using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.UnitTests.DBase.Fields
{
    [TestFixture]
    public class NumericFieldTests
    {
        [Test]
        public void CanGetNullValueFromFloatField()
        {
            var data = FieldTests.GetFieldData("Num Field", 'N', 10, 2);
            var field = Field.ParseMetaData(data);
            field.Parse(Encoding.ASCII.GetBytes(new string(' ', 10)));
            Assert.That(field.Value, Is.Null);
        }

        [Test]
        public void CanGetValueFromFloatField()
        {
            const double fieldValue = -84430.56;
            var data = FieldTests.GetFieldData("Num Field", 'N', 10, 2);
            var field = Field.ParseMetaData(data);
            field.Parse(Encoding.ASCII.GetBytes(string.Format("{0,10}", fieldValue)));
            Assert.That(field.Value, Is.EqualTo(fieldValue));
        }

        [Test]
        public void CanParseNumericField()
        {
            var data = FieldTests.GetFieldData("Num Field", 'N', 10, 2);

            var field = Field.ParseMetaData(data);
            Assert.That(field.Name, Is.EqualTo("Num Field"));
            Assert.That(field.Type, Is.EqualTo(typeof(double)));
            Assert.That(field.TypeName, Is.EqualTo("Numeric"));
            Assert.That(field.Length, Is.EqualTo(10));
            Assert.That(field.DecimalCount, Is.EqualTo(2));
        }

        [Test]
        public void WillGetAnExceptionIfNumericFieldIsLongerThanTwenty()
        {
            var data = FieldTests.GetFieldData("Num Field", 'N', 21, 4);
            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(data));
        }
    }
}
