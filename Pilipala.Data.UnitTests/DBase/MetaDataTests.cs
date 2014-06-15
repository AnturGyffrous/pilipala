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
    public class MetaDataTests
    {
        private const string _record1Field1 = "This is the first field in the first record.";

        private const string _record2Field1 = "This is the first field in the second record, but it has been deleted.";

        private const string _record3Field1 = "This is the first field in the third and final record.";

        private static byte[] GetBytes()
        {
            return new byte[]
                   {
                       15, // Year last updated
                       10, // Month last updated
                       21, // Day last updated
                       45, 1, 0, 0, // Record count
                       65, 0, // Header byte count
                       0, 1, // Record byte count
                       0, 0, // Reserved
                       1, // Incomplete transaction flag
                       1, // Encryption flag
                       0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // Reserved for dBASE IV in a multi-user environment
                       1, // Production MDX file flag
                       3, // Language driver ID
                       0 // Reserved
                   }
                .Concat(FieldTests.GetFieldData("FIRST FIELD", 'C', 255))
                .Concat(new byte[] { 0xd })
                .Concat(CharacterFieldTests.CreateStringFieldData(" " + _record1Field1, 256))
                .Concat(CharacterFieldTests.CreateStringFieldData("*" + _record2Field1, 256))
                .Concat(CharacterFieldTests.CreateStringFieldData(" " + _record3Field1, 256))
                .Concat(new byte[] { 0x1a })
                .ToArray();
        }

        [Test]
        public void CanParseHeader()
        {
            using (var stream = new MemoryStream(GetBytes()))
            {
                var metaData = MetaData.Initialise(stream);
                Assert.That(metaData.LastUpdated, Is.EqualTo(new DateTime(2015, 10, 21)));
                Assert.That(metaData.RecordsAffected, Is.EqualTo(301));
                Assert.That(metaData.RecordLength, Is.EqualTo(metaData.Fields.Sum(x => x.Length) + 1));
                Assert.That(metaData.IncompleteTransaction, Is.True);
                Assert.That(metaData.Encrypted, Is.True);
                Assert.That(metaData.ProductionMdx, Is.True);
                Assert.That(metaData.LanguageDriverID, Is.EqualTo(3));
                Assert.That(metaData.Fields.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void CanReadRecord()
        {
            using (var stream = new MemoryStream(GetBytes()))
            {
                var metaData = MetaData.Initialise(stream);
                Assert.That(metaData.Read(), Is.True);
            }
        }

        [Test]
        public void FieldValueIsNullBeforeReadingFirstRecord()
        {
            using (var stream = new MemoryStream(GetBytes()))
            {
                var data = MetaData.Initialise(stream);
                Assert.That(data.Fields[0].Value, Is.Null);
            }
        }

        [Test]
        public void ReadReturnsFalseIfThereAreNoMoreRecordsToRead()
        {
            using (var stream = new MemoryStream(GetBytes()))
            {
                var metaData = MetaData.Initialise(stream);
                Assert.That(metaData.Read(), Is.True);
                Assert.That(metaData.Read(), Is.True);
                Assert.That(metaData.Read(), Is.False);
            }
        }

        [Test]
        public void ReadSkipsTheRecordIfItIsDeleted()
        {
            using (var stream = new MemoryStream(GetBytes()))
            {
                var metaData = MetaData.Initialise(stream);
                Assert.That(metaData.Read(), Is.True);
                Assert.That(metaData.Fields[0].Value, Is.EqualTo(_record1Field1));

                Assert.That(metaData.Read(), Is.True);
                Assert.That(metaData.Fields[0].Value, Is.EqualTo(_record3Field1));
            }
        }

        [Test]
        public void ReadWillReturnFalseOnceTheEndOfFileMarkerHasBeenReached()
        {
            var lengthOfRecord = BitConverter.ToInt16(GetBytes(), 9);
            using (
                var stream =
                    new MemoryStream(GetBytes().Concat(CharacterFieldTests.CreateStringFieldData("Some random data beyond that end of file marker", lengthOfRecord)).ToArray()))
            {
                var metaData = MetaData.Initialise(stream);
                Assert.That(metaData.Read(), Is.True);
                Assert.That(metaData.Read(), Is.True);
                Assert.That(metaData.Read(), Is.False);
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
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Initialise(stream));
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
                Assert.Throws<ArgumentOutOfRangeException>(() => MetaData.Initialise(stream));
            }
        }

        [Test]
        public void WillGetAnExceptionIfTheHeaderDoesNotEndWithAFieldTerminator()
        {
            var headerBytes = GetBytes();
            using (var stream = new MemoryStream(headerBytes.Take(BitConverter.ToInt16(headerBytes, 7) - 2).ToArray()))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Initialise(stream));
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
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Initialise(stream));
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
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Initialise(stream));
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
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Initialise(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfThereIsNotEnoughHeaderData()
        {
            using (var stream = new MemoryStream(GetBytes().Take(25).ToArray()))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Initialise(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfThereIsNotFieldDefinitionData()
        {
            using (var stream = new MemoryStream(GetBytes().Take(50).ToArray()))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => MetaData.Initialise(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }
    }
}
