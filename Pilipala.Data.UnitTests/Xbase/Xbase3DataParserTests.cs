using System;
using System.IO;

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
            var generator = new Xbase3DataGenerator
                            {
                                IncompleteTransactionFlag = (byte)incompleteTransactionFlagValue, 
                                EncryptionFlag = (byte)encryptionFlagValue, 
                                MdxFlag = (byte)mdxFlagValue, 
                                LanguageDriverId = (byte)languageDriverId
                            };

            using (var stream = new MemoryStream(generator.GetData()))
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
            var generator = new Xbase3DataGenerator { LastUpdatedYear = (byte)year, LastUpdatedMonth = (byte)month, LastUpdatedDay = (byte)day };

            using (var stream = new MemoryStream(generator.GetData()))
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
            var generator = new Xbase3DataGenerator { RecordCount = -3 };

            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => Xbase3DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnExceptionIfTheRemainderOfTheHeaderLengthDividedByThirtyTwoIsNotOne()
        {
            var generator = new Xbase3DataGenerator { HeaderByteCount = 96 };
            using (var stream = new MemoryStream(generator.GetData()))
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
            var generator = new Xbase3DataGenerator { Version = 0 };
            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => Xbase3DataParser.Create(stream));
            }
        }
    }
}
