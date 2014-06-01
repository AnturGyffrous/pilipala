using System;

using NUnit.Framework;

using Pilipala.Data.DBase;

namespace Pilipala.Data.UnitTests.DBase
{
    [TestFixture]
    public class FieldTests
    {
        [Test]
        public void CanParseCharacterField()
        {
            var data = new byte[]
                       {
                           // Field name
                           (byte)'C',
                           (byte)'h',
                           (byte)'a',
                           (byte)'r',
                           (byte)' ',
                           (byte)'F',
                           (byte)'i',
                           (byte)'e',
                           (byte)'l',
                           (byte)'d',
                           0,
                           (byte)'C', // Field type
                           0, 0, 0, 0, // Reserved
                           50, // Field length
                           0, // Field decimal count
                           0, 0, // Reserved
                           0, // Work area ID
                           0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // Reserved
                           0 // Production MDX field flag
                       };
            var field = Field.Parse(data);

            Assert.That(field.Name, Is.EqualTo("Char Field"));
        }

        [Test]
        public void WillGetAnExceptionIfNotEnoughBytesHaveBeenProvided()
        {
            Assert.Throws<InvalidOperationException>(() => Field.Parse(new byte[] { 0, 1, 2, 3, 4, 5 }));
        }
    }
}
