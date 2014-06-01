using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Pilipala.Data.DBase;

namespace Pilipala.Data.UnitTests.DBase
{
    [TestFixture]
    public class DBaseHeaderParserTests
    {
        private byte[] GetHeaderBytes()
        {
            return new byte[]
                   {
                       15, // Year last updated
                       10, // Month last updated
                       21, // Day last updated
                       45, 1, 0, 0, // Record count
                       177, 3, // Header byte count
                       99, 2, // Record byte count
                       0, 0, // Reserved
                       1, // Incomplete transaction flag
                       1, // Encryption flag
                       0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // Reserved for dBASE IV in a multi-user environment
                       1, // Production MDX file flag
                       3, // Language driver ID
                       0 // Reserved
                   };
        }

        [Test]
        public void CanParseHeader()
        {
            using (var stream = new MemoryStream(GetHeaderBytes()))
            {
                var header = new DBaseHeaderParser(stream);
                Assert.That(header.LastUpdated, Is.EqualTo(new DateTime(2015, 10, 21)));
                Assert.That(header.RecordCount, Is.EqualTo(301));
                Assert.That(header.RecordLength, Is.EqualTo(611));
                Assert.That(header.IncompleteTransaction, Is.True);
                Assert.That(header.Encrypted, Is.True);
                Assert.That(header.ProductionMdx, Is.True);
                Assert.That(header.LanguageDriverID, Is.EqualTo(3));
            }
        }

        [Test]
        public void WillGetAnExceptionIfThereIsNotEnoughHeaderData()
        {
            using (var stream = new MemoryStream(GetHeaderBytes().Take(25).ToArray()))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => new DBaseHeaderParser(stream));
                Assert.That(exception.Message, Is.EqualTo(Resources.ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [TestCase(115, 10, 21)]
        [TestCase(15, 0, 21)]
        [TestCase(15, 13, 21)]
        [TestCase(15, 10, 0)]
        [TestCase(15, 10, 32)]
        public void WillGetAnExceptionIfLastModifiedIsInvalid(byte year, byte month, byte day)
        {
            var headerBytes = GetHeaderBytes();
            headerBytes[0] = year;
            headerBytes[1] = month;
            headerBytes[2] = day;
            using (var stream = new MemoryStream(headerBytes))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => new DBaseHeaderParser(stream));
                Assert.That(exception.Message, Is.EqualTo(Resources.ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfLastModifiedIsNotAValidDate()
        {
            var headerBytes = GetHeaderBytes();
            headerBytes[1] = 2;
            headerBytes[2] = 30; // 30th February!
            using (var stream = new MemoryStream(headerBytes))
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new DBaseHeaderParser(stream));
            }
        }
    }
}
