﻿using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Pilipala.Data.DBase;
using Pilipala.Data.Resources;

namespace Pilipala.Data.UnitTests.DBase
{
    [TestFixture]
    public class HeaderParserTests
    {
        private static byte[] GetHeaderBytes()
        {
            return new byte[]
                   {
                       15, // Year last updated
                       10, // Month last updated
                       21, // Day last updated
                       45, 1, 0, 0, // Record count
                       65, 0, // Header byte count
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
            using (var stream = new MemoryStream(GetHeaderBytes()))
            {
                var header = new HeaderParser(stream);
                Assert.That(header.LastUpdated, Is.EqualTo(new DateTime(2015, 10, 21)));
                Assert.That(header.RecordCount, Is.EqualTo(301));
                Assert.That(header.RecordLength, Is.EqualTo(255));
                Assert.That(header.IncompleteTransaction, Is.True);
                Assert.That(header.Encrypted, Is.True);
                Assert.That(header.ProductionMdx, Is.True);
                Assert.That(header.LanguageDriverID, Is.EqualTo(3));

                Assert.That(header.Fields.Count(), Is.EqualTo(1));
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
                var exception = Assert.Throws<InvalidOperationException>(() => new HeaderParser(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
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
                Assert.Throws<ArgumentOutOfRangeException>(() => new HeaderParser(stream));
            }
        }

        [Test]
        public void WillGetAnExceptionIfTheHeaderLengthIsLessThanOne()
        {
            var headerBytes = GetHeaderBytes();
            headerBytes[9] = 0;
            headerBytes[10] = 0;
            using (var stream = new MemoryStream(headerBytes))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => new HeaderParser(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfTheRemainderOfTheHeaderLengthDividedByThirtyTwoIsNotOne()
        {
            var headerBytes = GetHeaderBytes();
            var headerLength = BitConverter.GetBytes((short)96);
            headerBytes[7] = headerLength[0];
            headerBytes[8] = headerLength[1];
            using (var stream = new MemoryStream(headerBytes))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => new HeaderParser(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfTheRecordCountIsLessThanZero()
        {
            var headerBytes = GetHeaderBytes();
            var recordCount = BitConverter.GetBytes(-3);
            headerBytes[3] = recordCount[0];
            headerBytes[4] = recordCount[1];
            headerBytes[5] = recordCount[2];
            headerBytes[6] = recordCount[3];
            using (var stream = new MemoryStream(headerBytes))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => new HeaderParser(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfThereIsNotEnoughHeaderData()
        {
            using (var stream = new MemoryStream(GetHeaderBytes().Take(25).ToArray()))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => new HeaderParser(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfThereIsNotFieldDefinitionData()
        {
            using (var stream = new MemoryStream(GetHeaderBytes().Take(50).ToArray()))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => new HeaderParser(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }

        [Test]
        public void WillGetAnExceptionIfTheHeaderDoesNotEndWithAFieldTerminator()
        {
            var headerBytes = GetHeaderBytes();
            using (var stream = new MemoryStream(headerBytes.Take(headerBytes.Length - 1).ToArray()))
            {
                var exception = Assert.Throws<InvalidOperationException>(() => new HeaderParser(stream));
                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.DBaseDataReader_InvalidFormat));
            }
        }
    }
}
