using System;

using FluentAssertions;

using NSubstitute;

using Pilipala.Data.Xbase;
using Pilipala.Tests.Extensions;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;

using Xunit;

namespace Pilipala.Data.UnitTests.Xbase
{
    public class XbaseDataReaderTests
    {
        private const string _noDataHasBeenReadExceptionMessage =
            "No data has been read. You must advance the cursor past the beginning of the file by calling Read() or ReadAsync() before inspecting the data.";

        private readonly IFixture _fixture;

        public XbaseDataReaderTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _fixture.Freeze<IXbaseDataParser>();
        }

        [Fact]
        public void DepthShouldBeZeroAfterRead()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            reader.Read();

            // Assert
            reader.Depth.Should().Be(0);
        }

        [Fact]
        public void DepthShouldThrowInvalidOperationExceptionIfNoDataHasBeenRead()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            Action depth = () => reader.Depth.IgnoreUnusedVariable();

            // Assert
            depth
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(_noDataHasBeenReadExceptionMessage);
        }

        [Fact]
        public void HasRowsShouldReturnFalseIfNoRecordsExistInRecordset()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            reader.Read();

            // Assert
            reader.HasRows.Should().BeFalse();
        }

        [Fact]
        public void HasRowsShouldReturnTrueIfRecordsExistInRecordset()
        {
            // Arrange
            var parser = _fixture.Create<IXbaseDataParser>();
            var reader = _fixture.Create<XbaseDataReader>();

            parser.RecordsAffected.Returns(3);

            // Act
            reader.Read();

            // Assert
            reader.HasRows.Should().BeTrue();
        }

        [Fact]
        public void HasRowsShouldThrowInvalidOperationExceptionIfNoDataHasBeenRead()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            Action depth = () => reader.HasRows.IgnoreUnusedVariable();

            // Assert
            depth
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(_noDataHasBeenReadExceptionMessage);
        }

        [Fact]
        public void NextResultShouldThrowNotImplementedException()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            Action nextResult = () => reader.NextResult();

            // Assert
            nextResult.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void ReadShouldReturnFalseWhenThereAreNoMoreRecordsToRead()
        {
            // Arrange
            var parser = _fixture.Create<IXbaseDataParser>();
            var reader = _fixture.Create<XbaseDataReader>();

            // Arrange
            parser.Read().Returns(true, true, false);

            // Act
            var firstRead = reader.Read();
            var secondRead = reader.Read();
            var thirdRead = reader.Read();

            // Assert
            firstRead.Should().BeTrue();
            secondRead.Should().BeTrue();
            thirdRead.Should().BeFalse();
            parser.Received(3).Read();
        }

        [Fact]
        public void ReadShouldReturnTrueIfThereAreRecordsInTheDatabase()
        {
            // Arrange
            var parser = _fixture.Create<IXbaseDataParser>();
            var reader = _fixture.Create<XbaseDataReader>();

            // Arrange
            parser.Read().Returns(true);

            // Act
            var result = reader.Read();

            // Assert
            result.Should().BeTrue();
            parser.Received().Read();
        }

        [Fact]
        public void RecordsAffectedShouldBeZeroAfterReaderIsClosed()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            reader.Close();

            // Assert
            reader.RecordsAffected.Should().Be(0);
        }

        [Fact]
        public void RecordsAffectedShouldReturnRecordCountAfterRead()
        {
            // Arrange
            var parser = _fixture.Create<IXbaseDataParser>();
            var reader = _fixture.Create<XbaseDataReader>();

            // Arrange
            parser.RecordsAffected.Returns(3);

            // Act
            reader.Read();

            // Assert
            reader.RecordsAffected.Should().Be(3);
            parser.Received().RecordsAffected.IgnoreUnusedVariable();
        }

        [Fact]
        public void RecordsAffectedShouldThrowInvalidOperationExceptionIfNoDataHasBeenRead()
        {
            // Arrange
            var reader = _fixture.Create<XbaseDataReader>();

            // Act
            Action recordsAffected = () => reader.RecordsAffected.IgnoreUnusedVariable();

            // Assert
            recordsAffected
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(_noDataHasBeenReadExceptionMessage);
        }
    }
}
