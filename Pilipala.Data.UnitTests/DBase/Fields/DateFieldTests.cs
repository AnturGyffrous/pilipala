using System;
using System.Text;

using NUnit.Framework;

using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.UnitTests.DBase.Fields
{
    [TestFixture]
    public class DateFieldTests
    {
        [Test]
        public void CanGetValueFromDateField()
        {
            var fieldValue = new DateTime(2015, 10, 21);
            var data = FieldTests.GetFieldData("Date Field", 'D', 8);
            var field = Field.ParseMetaData(data);
            field.Parse(Encoding.ASCII.GetBytes(fieldValue.ToString("yyyyMMdd")));
            Assert.That(field.Value, Is.EqualTo(fieldValue));
        }

        [Test]
        public void CanParseDateField()
        {
            var data = FieldTests.GetFieldData("Date Field", 'D', 8);

            var field = Field.ParseMetaData(data);
            Assert.That(field.Name, Is.EqualTo("Date Field"));
            Assert.That(field.Type, Is.EqualTo(typeof(DateTime)));
            Assert.That(field.TypeName, Is.EqualTo("Date"));
            Assert.That(field.Length, Is.EqualTo(8));
            Assert.That(field.DecimalCount, Is.EqualTo(0));
        }

        [Test]
        public void WillGetAnExceptionIfDateFieldDoesNotHaveDecimalLengthOfZero()
        {
            var data = FieldTests.GetFieldData("Date Field", 'D', 8, 4);
            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(data));
        }

        [Test]
        public void WillGetAnExceptionIfDateFieldDoesNotHaveLengthOfEight()
        {
            var data = FieldTests.GetFieldData("Date Field", 'D', 10);
            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(data));
        }
    }
}
