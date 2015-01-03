using System.IO;

using FluentAssertions;

using Pilipala.Data.Xbase.Xbase3;

using Xunit;

namespace Pilipala.Data.UnitTests.Xbase.Xbase3
{
    public class FieldDescriptionParserTests
    {
        [Fact]
        public void CanParseXbase3FieldDefinitions()
        {
            var generator = new DataGenerator();
            using (var stream = new MemoryStream(generator.GetData()))
            {
                var parser = DataParser.Create(stream);
                parser.Fields.Should().HaveCount(1);
            }
        }
    }
}
