using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Pilipala.Data.DBase.Fields;

namespace Pilipala.Data.UnitTests.DBase.Fields
{
    [TestFixture]
    public class CharacterFieldTests
    {
        internal static byte[] CreateStringFieldData(string value, int length)
        {
            return Encoding.ASCII.GetBytes(value).Concat(Enumerable.Repeat((byte)32, length)).Take(length).ToArray();
        }

        [Test]
        public void CanGetValueFromCharacterField()
        {
            const string fieldValue = "This is an example of a Character record";
            var data = FieldTests.GetFieldData("Char Field", 'C', 50);
            var field = Field.ParseMetaData(data);
            field.Parse(CreateStringFieldData(fieldValue, 50), 0);
            Assert.That(field.Value, Is.EqualTo(fieldValue));
        }

        [Test]
        public void CanParseCharacterField()
        {
            var data = FieldTests.GetFieldData("Char Field", 'C', 50, 0, 1, true);
            var field = Field.ParseMetaData(data);

            Assert.That(field.Name, Is.EqualTo("Char Field"));
            Assert.That(field.Type, Is.EqualTo(typeof(string)));
            Assert.That(field.TypeName, Is.EqualTo("Character"));
            Assert.That(field.Length, Is.EqualTo(50));
            Assert.That(field.DecimalCount, Is.EqualTo(0));
            Assert.That(field.WorkAreaID, Is.EqualTo(1));
            Assert.That(field.ProductionMdx, Is.True);
        }

        [Test]
        public void WillGetAnExceptionIfCharFieldDoesNotHaveDecimalLengthOfZero()
        {
            var data = FieldTests.GetFieldData("Char Field", 'C', 50, 3);
            Assert.Throws<InvalidOperationException>(() => Field.ParseMetaData(data));
        }
    }
}
