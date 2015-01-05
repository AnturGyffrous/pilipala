using System;
using System.IO;

using FluentAssertions;

using Pilipala.Data.Xbase.Xbase3;

using Xunit;
using Xunit.Extensions;

namespace Pilipala.Data.UnitTests.Xbase.Xbase3
{
    public class HeaderParserTests
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
        public void CanParseXbase3Header(
            int incompleteTransactionFlagValue, 
            bool expectedIncompleteTransactionFlag, 
            int encryptionFlagValue, 
            bool expectedEncryptionFlag, 
            int mdxFlagValue, 
            bool expectedMdxFlag, 
            int languageDriverId)
        {
            var generator = new DataGenerator
                            {
                                IncompleteTransactionFlag = (byte)incompleteTransactionFlagValue, 
                                EncryptionFlag = (byte)encryptionFlagValue, 
                                MdxFlag = (byte)mdxFlagValue, 
                                LanguageDriverId = (byte)languageDriverId
                            };

            using (var stream = new MemoryStream(generator.GetData()))
            {
                var parser = DataParser.Create(stream);
                parser.LastUpdated.Should().Be(new DateTime(2015, 10, 21));
                parser.RecordsAffected.Should().Be(generator.RecordCount);
                parser.RecordLength.Should().Be(generator.RecordLength);
                parser.IncompleteTransaction.Should().Be(expectedIncompleteTransactionFlag);
                parser.Encrypted.Should().Be(expectedEncryptionFlag);
                parser.Mdx.Should().Be(expectedMdxFlag);
                parser.LanguageDriverID.Should().Be(languageDriverId);
            }
        }

        [Fact]
        public void ParserHasNoPublicConstructors()
        {
            typeof(DataParser).GetConstructors().Should().HaveCount(0);
        }

        [Theory]
        [InlineData(115, 0, 0)]
        [InlineData(115, 10, 0)]
        [InlineData(115, 13, 0)]
        [InlineData(115, 10, 32)]
        [InlineData(115, 2, 29)]
        public void WillGetAnExceptionIfLastModifiedDateIsInvalid(int year, int month, int day)
        {
            var generator = new DataGenerator { LastUpdatedYear = (byte)year, LastUpdatedMonth = (byte)month, LastUpdatedDay = (byte)day };

            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnExceptionIfLessThan32BytesCanBeReadFromTheStream()
        {
            using (var stream = new MemoryStream())
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnExceptionIfTheRecordCountIsLessThanZero()
        {
            var generator = new DataGenerator { RecordCount = -3 };

            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        [InlineData(1)]
        public void WillGetAnExceptionIfTheRecordLengthIsLessThanTwo(int recordLength)
        {
            var generator = new DataGenerator { RecordLength = (short)recordLength };

            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnExceptionIfTheRemainderOfTheHeaderLengthDividedByThirtyTwoIsNotOne()
        {
            var generator = new DataGenerator { HeaderByteCount = 96 };
            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnExceptionIfTheStreamIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => DataParser.Create(null));
        }

        [Fact]
        public void WillGetAnExceptionIfTheVersionIsNotSupported()
        {
            var generator = new DataGenerator { Version = 0 };
            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }
    }
}
