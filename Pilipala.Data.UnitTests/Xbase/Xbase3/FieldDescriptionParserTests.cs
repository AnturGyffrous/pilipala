using System;
using System.IO;
using System.Linq;

using FluentAssertions;

using Pilipala.Data.Xbase.Xbase3;

using Xunit;
using Xunit.Extensions;

namespace Pilipala.Data.UnitTests.Xbase.Xbase3
{
    public class FieldDescriptionParserTests
    {
        [Fact]
        public void CanParseCharacterField()
        {
            var generator = new DataGenerator();
            using (var stream = new MemoryStream(generator.GetData()))
            {
                var parser = DataParser.Create(stream);
                var numericField = parser.Fields.ElementAt(1);

                numericField.Length.Should().Be(DataGenerator.TitleFieldLength);
                numericField.Name.Should().Be(DataGenerator.TitleFieldName);
                numericField.Type.Should().Be(typeof(string));
                numericField.TypeName.Should().Be("Character");
            }
        }

        [Fact]
        public void CanParseDateField()
        {
            var generator = new DataGenerator();
            using (var stream = new MemoryStream(generator.GetData()))
            {
                var parser = DataParser.Create(stream);
                var numericField = parser.Fields.ElementAt(4);

                numericField.Length.Should().Be(DataGenerator.ReleasedFieldLength);
                numericField.Name.Should().Be(DataGenerator.ReleasedFieldName);
                numericField.Type.Should().Be(typeof(DateTime));
                numericField.TypeName.Should().Be("Date");
            }
        }

        [Fact]
        public void CanParseFloatField()
        {
            var generator = new DataGenerator();
            using (var stream = new MemoryStream(generator.GetData()))
            {
                var parser = DataParser.Create(stream);
                var numericField = parser.Fields.ElementAt(2);

                numericField.Length.Should().Be(DataGenerator.RatingFieldLength);
                numericField.Name.Should().Be(DataGenerator.RatingFieldName);
                numericField.Type.Should().Be(typeof(double));
                numericField.TypeName.Should().Be("Float");
            }
        }

        [Fact]
        public void CanParseLogicalField()
        {
            var generator = new DataGenerator();
            using (var stream = new MemoryStream(generator.GetData()))
            {
                var parser = DataParser.Create(stream);
                var numericField = parser.Fields.ElementAt(3);

                numericField.Length.Should().Be(DataGenerator.Top250FieldLength);
                numericField.Name.Should().Be(DataGenerator.Top250FieldName);
                numericField.Type.Should().Be(typeof(bool));
                numericField.TypeName.Should().Be("Logical");
            }
        }

        [Fact]
        public void CanParseNumericField()
        {
            var generator = new DataGenerator();
            using (var stream = new MemoryStream(generator.GetData()))
            {
                var parser = DataParser.Create(stream);
                var numericField = parser.Fields.ElementAt(0);

                numericField.Length.Should().Be(DataGenerator.IdFieldLength);
                numericField.Name.Should().Be(DataGenerator.IdFieldName);
                numericField.Type.Should().Be(typeof(double));
                numericField.TypeName.Should().Be("Numeric");
            }
        }

        [Fact]
        public void CanParseXbase3FieldDefinitions()
        {
            var generator = new DataGenerator();
            using (var stream = new MemoryStream(generator.GetData()))
            {
                var parser = DataParser.Create(stream);
                parser.Fields.Should().HaveCount(generator.Fields.Count());
            }
        }

        [Theory]
        [InlineData(7)]
        [InlineData(9)]
        public void WillGetAnErrorIfDateFieldHasLengthOtherThan8(int fieldLength)
        {
            var generator = new DataGenerator();
            generator.Fields.ElementAt(4)[16] = (byte)fieldLength;
            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnErrorIfFieldIsDefinedWithUnknownCode()
        {
            var generator = new DataGenerator();
            generator.Fields.ElementAt(0)[11] = (byte)'A';
            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }

        [Theory]
        [InlineData(19)]
        [InlineData(21)]
        public void WillGetAnErrorIfFloatFieldHasLengthOtherThan20(int fieldLength)
        {
            var generator = new DataGenerator();
            generator.Fields.ElementAt(2)[16] = (byte)fieldLength;
            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnErrorIfLogicalFieldHasLengthGreaterThan1()
        {
            var generator = new DataGenerator();
            generator.Fields.ElementAt(3)[16] = 2;
            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }

        [Fact]
        public void WillGetAnErrorIfNumericFieldHasLengthGreaterThan20()
        {
            var generator = new DataGenerator();
            generator.Fields.ElementAt(0)[16] = 25;
            using (var stream = new MemoryStream(generator.GetData()))
            {
                Assert.Throws<InvalidOperationException>(() => DataParser.Create(stream));
            }
        }
    }
}
