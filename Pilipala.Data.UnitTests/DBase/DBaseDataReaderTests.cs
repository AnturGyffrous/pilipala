using System;
using System.Data.Common;
using System.IO;

using NUnit.Framework;

using Pilipala.Data.DBase;
using Pilipala.Data.Resources;

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
                using (var reader = DBaseDataReader.Create(stream) as DbDataReader)
                {
                    Assert.That(reader.IsClosed, Is.False);
                    Assert.That(reader.HasRows, Is.True);
                    Assert.That(reader.Depth, Is.EqualTo(0));
                    Assert.That(reader.RecordsAffected, Is.EqualTo(1));
                    Assert.That(reader.FieldCount, Is.EqualTo(57));

                    //Assert.That(reader.Read(), Is.True);
                }
            }
        }

        [Test]
        public void CannotCreateDBaseDataReaderWithNonDBaseData()
        {
            using (var stream = new MemoryStream(new byte[] { 2 }))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => DBaseDataReader.Create(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void CannotCreateDBaseDataReaderWithNullLengthStream()
        {
            using (var stream = new MemoryStream())
            {
                var exception = Assert.Throws<InvalidOperationException>(() => DBaseDataReader.Create(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void DBaseDataReaderDerivesFromTheCorrectType()
        {
            var reader = typeof(DBaseDataReader);
            Assert.That(reader.BaseType, Is.EqualTo(typeof(DbDataReader)));
            Assert.That(reader.GetInterfaces(), Has.Member(typeof(IDisposable)));
        }

        [Test]
        public void ReaderIsMarkedAsClosedAfterDisposing()
        {
            using (var stream = new MemoryStream(Properties.Resources.example))
            {
                var reader = DBaseDataReader.Create(stream) as DbDataReader;
                reader.Dispose();
                Assert.That(reader.IsClosed, Is.True);
            }
        }
    }
}
