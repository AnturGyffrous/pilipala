using FluentAssertions;

using NSubstitute;

using Pilipala.Data.Xbase;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;

using Xunit;

namespace Pilipala.Data.UnitTests.Xbase
{
    public class XbaseDataReaderLifetimeTests
    {
        private readonly IFixture _fixture;

        public XbaseDataReaderLifetimeTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _fixture.Freeze<IXbaseDataParser>();
        }

        [Fact]
        public void CloseShouldBeCalledWhenDisposing()
        {
            // Arrange
            var parser = _fixture.Create<IXbaseDataParser>();
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            reader.Dispose();

            // Assert
            parser.Received().Close();
        }

        [Fact]
        public void IsClosedShouldBeFalseBeforeReaderIsClosed()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Assert
            reader.IsClosed.Should().BeFalse();
        }

        [Fact]
        public void IsClosedShouldBeTrueAfterReaderIsClosed()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            reader.Close();

            // Assert
            reader.IsClosed.Should().BeTrue();
        }
    }
}
