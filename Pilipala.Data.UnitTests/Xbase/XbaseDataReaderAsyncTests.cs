using System;
using System.Threading.Tasks;

using FluentAssertions;

using NSubstitute;

using Pilipala.Data.Xbase;
using Pilipala.Tests.Extensions;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;

using Xunit;

namespace Pilipala.Data.UnitTests.Xbase
{
    public class XbaseDataReaderAsyncTests
    {
        private readonly IFixture _fixture;

        public XbaseDataReaderAsyncTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _fixture.Freeze<IXbaseDataParser>();
        }

        [Fact]
        public async void DepthShouldBeZeroAfterReadAsync()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            await reader.ReadAsync();

            // Assert
            reader.Depth.Should().Be(0);
        }

        [Fact]
        public void NextResultAsyncShouldThrowNotSupportedException()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            Func<Task<bool>> nextResultAsync = reader.NextResultAsync;

            // Assert
            nextResultAsync.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public async void ReadAsyncShouldReturnFalseWhenThereAreNoMoreRecordsToRead()
        {
            // Arrange
            var parser = _fixture.Create<IXbaseDataParser>();
            var reader = _fixture.Create<XbaseDataReader>();

            // Arrange
            parser.ReadAsync().Returns(Task.FromResult(true), Task.FromResult(true), Task.FromResult(false));

            // Act
            var firstRead = await reader.ReadAsync();
            var secondRead = await reader.ReadAsync();
            var thirdRead = await reader.ReadAsync();

            // Assert
            firstRead.Should().BeTrue();
            secondRead.Should().BeTrue();
            thirdRead.Should().BeFalse();
            parser.Received(3).ReadAsync().IgnoreAwaitWarning();
        }

        [Fact]
        public async void ReadAsyncShouldReturnTrueIfThereAreRecordsInTheDatabase()
        {
            // Arrange
            var parser = _fixture.Create<IXbaseDataParser>();
            var reader = _fixture.Create<XbaseDataReader>();

            // Arrange
            parser.ReadAsync().Returns(Task.FromResult(true));

            // Act
            var result = await reader.ReadAsync();

            // Assert
            result.Should().BeTrue();
            parser.Received().ReadAsync().IgnoreAwaitWarning();
        }

        [Fact]
        public async void RecordsAffectedShouldReturnRecordCountAfterReadAsync()
        {
            // Arrange
            var parser = _fixture.Create<IXbaseDataParser>();
            var reader = _fixture.Create<XbaseDataReader>();

            // Arrange
            parser.RecordsAffected.Returns(3);

            // Act
            await reader.ReadAsync();

            // Assert
            reader.RecordsAffected.Should().Be(3);
            parser.Received().RecordsAffected.IgnoreUnusedVariable();
        }
    }
}
