using System;
using System.Data.Common;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace Pilipala.Data.UnitTests
{
    [TestFixture]
    public class DBaseDataReaderTests
    {
        [Test]
        public void DBaseDataReaderDerivesFromTheCorrectType()
        {
            var reader = typeof(DBaseDataReader);
            Assert.That(reader.BaseType, Is.EqualTo(typeof(DbDataReader)));
            Assert.That(reader.GetInterfaces(), Has.Member(typeof(IDisposable)));
        }

        [Test]
        public void CannotCreateDBaseDataReaderWithNullLengthStream()
        {
            using (var stream = new MemoryStream())
            {
                Assert.Throws<InvalidOperationException>(() => DBaseDataReader.Create(stream));
            }
        }

        [Test]
        public void CannotCreateDBaseDataReaderWithNonDBaseData()
        {
            using (var stream = new MemoryStream(new byte[] { 2 }))
            {
                Assert.Throws<InvalidOperationException>(() => DBaseDataReader.Create(stream));
            }
        }

        [TestCase(0, 0, 0, 1)]
        [TestCase(115, 0, 0, 2)]
        [TestCase(115, 0, 0, 3)]
        [TestCase(115, 0, 1, 4)]
        [TestCase(115, 13, 1, 4)]
        [TestCase(115, 10, 0, 4)]
        [TestCase(115, 10, 32, 4)]
        public void CannotCreateDBaseDataReaderWithInvalidLastModifiedDate(byte year, byte month, byte day, int length)
        {
            using (var stream = new MemoryStream(new byte[] { 3, year, month, day }.Take(length).ToArray()))
            {
                Assert.Throws<InvalidOperationException>(() => DBaseDataReader.Create(stream));
            }
        }

        [Test]
        public void CannotCreateDBaseDataReaderWhenLastModifiedDateIsNotValid()
        {
            using (var stream = new MemoryStream(new byte[] { 3, 115, 2, 29 }))
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => DBaseDataReader.Create(stream));
            }
        }

        [Test]
        public void CannotCreateDBaseDataReaderIfThereAreLessThanFourBytesInTheRecordCount()
        {
            using (var stream = new MemoryStream(new byte[] { 3, 115, 10, 21, 1, 1, 1 }))
            {
                Assert.Throws<InvalidOperationException>(() => DBaseDataReader.Create(stream));
            }
        }

        [Test]
        public void CanReadDbfTable()
        {
            using (var stream = new MemoryStream(Properties.Resources.example))
            {
                var reader = DBaseDataReader.Create(stream);
                Assert.That(reader.IsClosed, Is.False);
                Assert.That(reader.Version, Is.EqualTo(3));
                Assert.That(reader.LastUpdated, Is.EqualTo(new DateTime(1901, 4, 24, 0, 0, 0, DateTimeKind.Utc)));
                Assert.That(reader.HasRows, Is.True);

                reader.Dispose();
                Assert.That(reader.IsClosed, Is.True);
            }
        }
    }
}
