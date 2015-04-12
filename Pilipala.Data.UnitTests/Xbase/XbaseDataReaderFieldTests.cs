using System;

using FluentAssertions;

using NSubstitute;

using Pilipala.Data.UnitTests.Properties;
using Pilipala.Data.Xbase;
using Pilipala.Tests.Extensions;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;

using Xunit;

namespace Pilipala.Data.UnitTests.Xbase
{
    public class XbaseDataReaderFieldTests
    {
        private readonly IFixture _fixture;

        public XbaseDataReaderFieldTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _fixture.Freeze<IXbaseDataParser>();
        }

        [Fact]
        public void FieldCountShouldReturnNumberOfFieldsAfterRead()
        {
            // Arrange
            var parser = _fixture.Create<IXbaseDataParser>();
            var reader = _fixture.Create<XbaseDataReader>();

            parser.Fields.Returns(new[] { Substitute.For<IField>(), Substitute.For<IField>(), Substitute.For<IField>() });

            // Act
            reader.Read();

            // Assert
            reader.FieldCount.Should().Be(3);
        }

        [Fact]
        public void FieldCountShouldThrowInvalidOperationExceptionIfNoDataHasBeenRead()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            Action depth = () => reader.FieldCount.IgnoreUnusedVariable();

            // Assert
            depth
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(Resources.NoDataHasBeenReadExceptionMessage);
        }
    }
}
