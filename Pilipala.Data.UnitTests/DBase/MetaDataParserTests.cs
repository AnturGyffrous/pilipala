using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Pilipala.Data.DBase;
using Pilipala.Data.Resources;
using Pilipala.Data.UnitTests.DBase.Fields;

namespace Pilipala.Data.UnitTests.DBase
{
    [TestFixture]
    public class MetaDataParserTests
    {
        private static byte[] GetBytes()
        {
            return new byte[]
                   {
                       15, // Year last updated
                       10, // Month last updated
                       21, // Day last updated
                       45, 1, 0, 0, // Record count
                       65, 0, // MetaData byte count
                       255, 0, // Record byte count
                       0, 0, // Reserved
                       1, // Incomplete transaction flag
                       1, // Encryption flag
                       0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // Reserved for dBASE IV in a multi-user environment
                       1, // Production MDX file flag
                       3, // Language driver ID
                       0 // Reserved
                   }
                .Concat(FieldTests.GetFieldData("FIRST FIELD", 'C', 255))
                .Concat(new byte[] { 13 })
                .ToArray();
        }

        [Test]
        public void CanParseHeader()
        {
            using (var stream = new MemoryStream(GetBytes()))
            {
                var header = MetaData.Parse(stream);
                Assert.That(header.LastUpdated, Is.EqualTo(new DateTime(2015, 10, 21)));
                Assert.That(header.RecordsAffected, Is.EqualTo(301));
                Assert.That(header.RecordLength, Is.EqualTo(255));
                Assert.That(header.IncompleteTransaction, Is.True);
                Assert.That(header.Encrypted, Is.True);
                Assert.That(header.ProductionMdx, Is.True);
                Assert.That(header.LanguageDriverID, Is.EqualTo(3));
                Assert.That(header.Fields.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void FieldValueIsNullBeforeReadingFirstRecord()
        {
            using (var stream = new MemoryStream(GetBytes()))
            {
                var data = MetaData.Parse(stream);
                Assert.That(data.Fields[0].Value, Is.Null);
            }
        }

        [TestCase(115, 10, 21)]
        [TestCase(15, 0, 21)]
        [TestCase(15, 13, 21)]
        [TestCase(15, 10, 0)]
        [TestCase(15, 10, 32)]
        public void WillGetAnExceptionIfLastModifiedIsInvalid(byte year, byte month, byte day)
        {
            var headerBytes = GetBytes();
            headerBytes[0] = year;
            headerBytes[1] = month;
            headerBytes[2] = day;
            using (var stream = new MemoryStream(headerBytes))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Parse(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfLastModifiedIsNotAValidDate()
        {
            var headerBytes = GetBytes();
            headerBytes[1] = 2;
            headerBytes[2] = 30; // 30th February!
            using (var stream = new MemoryStream(headerBytes))
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => MetaData.Parse(stream));
            }
        }

        [Test]
        public void WillGetAnExceptionIfTheHeaderDoesNotEndWithAFieldTerminator()
        {
            var headerBytes = GetBytes();
            using (var stream = new MemoryStream(headerBytes.Take(headerBytes.Length - 1).ToArray()))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Parse(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfTheHeaderLengthIsLessThanOne()
        {
            var headerBytes = GetBytes();
            headerBytes[9] = 0;
            headerBytes[10] = 0;
            using (var stream = new MemoryStream(headerBytes))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Parse(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfTheRecordCountIsLessThanZero()
        {
            var headerBytes = GetBytes();
            var recordCount = BitConverter.GetBytes(-3);
            headerBytes[3] = recordCount[0];
            headerBytes[4] = recordCount[1];
            headerBytes[5] = recordCount[2];
            headerBytes[6] = recordCount[3];
            using (var stream = new MemoryStream(headerBytes))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Parse(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfTheRemainderOfTheHeaderLengthDividedByThirtyTwoIsNotOne()
        {
            var headerBytes = GetBytes();
            var headerLength = BitConverter.GetBytes((short)96);
            headerBytes[7] = headerLength[0];
            headerBytes[8] = headerLength[1];
            using (var stream = new MemoryStream(headerBytes))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Parse(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfThereIsNotEnoughHeaderData()
        {
            using (var stream = new MemoryStream(GetBytes().Take(25).ToArray()))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Parse(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfThereIsNotFieldDefinitionData()
        {
            using (var stream = new MemoryStream(GetBytes().Take(50).ToArray()))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Parse(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }
    }
}
