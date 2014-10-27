using System;
using System.IO;
using System.Linq;

using FluentAssertions;

using Pilipala.Data.Xbase;

using Xunit;
using Xunit.Extensions;

namespace Pilipala.Data.UnitTests.Xbase
{
    public class Xbase3DataParserTests
    {
        [Theory]
        [InlineData(0, false, 0, false, 0, false, 0)]
        [InlineData(1, true, 0, false, 0, false, 0)]
        [InlineData(11, true, 0, false, 0, false, 0)]
        [InlineData(0, false, 1, true, 0, false, 0)]
        [InlineData(0, false, 5, true, 0, false, 0)]
        [InlineData(0, false, 0, false, 1, true, 0)]
        [InlineData(0, false, 0, false, 55, true, 0)]
        [InlineData(11, true, 5, true, 55, true, 21)]
        public void CanParseXbase3DataStream(
            int incompleteTransactionFlagValue, 
            bool expectedIncompleteTransactionFlag, 
            int encryptionFlagValue, 
            bool expectedEncryptionFlag, 
            int mdxFlagValue, 
            bool expectedMdxFlag, 
            int languageDriverId)
        {
            var data = new byte[]
                       {
                           3, // Version number
                           115, 10, 21, // Last updated date without century YYMMDD, valid year interval is 0x00 - 0xff, add to base year of 1900 for interval 1900 - 2155.
                           0, 0, 0, 0, // Record count (little endian)
                           33, 0, // Header byte count (little endian)
                           0, 0, // Sum of lengths of all fields + 1 for deletion flag (little endian)
                           0, 0, // Reserved
                           (byte)incompleteTransactionFlagValue, // Incomplete transaction flag
                           (byte)encryptionFlagValue, // Encryption flag
                           0, 0, 0, 0, // Free record thread (reserved for LAN only)
                           0, 0, 0, 0, 0, 0, 0, 0, // Reserved for multi-user dBASE ( dBASE III+ - )
                           (byte)mdxFlagValue, // MDX flag (dBASE IV)
                           (byte)languageDriverId, // Language driver ID
                           0, // Reserved
                           0xd // Header terminator
                       };
            using (var stream = new MemoryStream(data))
            {
                var parser = Xbase3DataParser.Create(stream);
                parser.LastUpdated.Should().Be(new DateTime(2015, 10, 21));
                parser.RecordsAffected.Should().Be(0);
                parser.RecordLength.Should().Be(0);
                parser.IncompleteTransaction.Should().Be(expectedIncompleteTransactionFlag);
                parser.Encrypted.Should().Be(expectedEncryptionFlag);
                parser.Mdx.Should().Be(expectedMdxFlag);
                parser.LanguageDriverID.Should().Be(languageDriverId);
            }
        }

        [Fact]
        public void ParserHasNoPublicConstructors()
        {
            typeof(Xbase3DataParser).GetConstructors().Should().HaveCount(0);
        }

        [Theory]
        [InlineData(115, 0, 0)]
        [InlineData(115, 10, 0)]
        [InlineData(115, 13, 0)]
        [InlineData(115, 10, 32)]
        [InlineData(115, 2, 29)]
        public void WillGetAnExceptionIfLastModifiedDateIsInvalid(int year, int month, int day)
        {
            using (var stream = new MemoryStream(new[] { (byte)3, (byte)year, (byte)month, (byte)day }.Concat(new byte[28]).ToArray()))
            {
                Assert.Throws<InvalidOperationException>(() => Xbase3DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnExceptionIfLessThan32BytesCanBeReadFromTheStream()
        {
            using (var stream = new MemoryStream())
            {
                Assert.Throws<InvalidOperationException>(() => Xbase3DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnExceptionIfTheRecordCountIsLessThanZero()
        {
            var recordCount = BitConverter.GetBytes(-3);
            var data = new byte[] { 3, 115, 10, 21, recordCount[0], recordCount[1], recordCount[2], recordCount[3] }.Concat(new byte[24]).ToArray();
            using (var stream = new MemoryStream(data))
            {
                Assert.Throws<InvalidOperationException>(() => Xbase3DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnExceptionIfTheRemainderOfTheHeaderLengthDividedByThirtyTwoIsNotOne()
        {
            var headerLength = BitConverter.GetBytes((short)96);
            var data = new byte[] { 3, 115, 10, 21, 0, 0, 0, 0, headerLength[0], headerLength[1] }.Concat(new byte[22]).ToArray();
            using (var stream = new MemoryStream(data))
            {
                Assert.Throws<InvalidOperationException>(() => Xbase3DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnExceptionIfTheStreamIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Xbase3DataParser.Create(null));
        }

        [Fact]
        public void WillGetAnExceptionIfTheVersionIsNotSupported()
        {
            using (var stream = new MemoryStream(new byte[32]))
            {
                Assert.Throws<InvalidOperationException>(() => Xbase3DataParser.Create(stream));
            }
        }
    }
}
