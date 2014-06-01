using System;
using System.Data.Common;
using System.IO;

using NUnit.Framework;

using Pilipala.Data.DBase;

namespace Pilipala.Data.UnitTests.DBase
{
    [TestFixture]
    public class DBaseDataReaderTests
    {
        [Test]
        public void CanReadDbfTable()
        {
            using (var stream = new MemoryStream(Properties.Resources.example))
            {
                var reader = DBaseDataReader.Create(stream) as DbDataReader;
                Assert.That(reader.IsClosed, Is.False);
                Assert.That(reader.HasRows, Is.True);

                reader.Dispose();
                Assert.That(reader.IsClosed, Is.True);
            }
        }

        [Test]
        public void CannotCreateDBaseDataReaderWithNonDBaseData()
        {
            using (var stream = new MemoryStream(new byte[] { 2 }))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => DBaseDataReader.Create(stream));
                Assert.That(exception.Message, Is.EqualTo(Resources.ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void CannotCreateDBaseDataReaderWithNullLengthStream()
        {
            using (var stream = new MemoryStream())
            {
                var exception = Assert.Throws<InvalidOperationException>(() => DBaseDataReader.Create(stream));
                Assert.That(exception.Message, Is.EqualTo(Resources.ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void DBaseDataReaderDerivesFromTheCorrectType()
        {
            var reader = typeof(DBaseDataReader);
            Assert.That(reader.BaseType, Is.EqualTo(typeof(DbDataReader)));
            Assert.That(reader.GetInterfaces(), Has.Member(typeof(IDisposable)));
        }
    }
}
