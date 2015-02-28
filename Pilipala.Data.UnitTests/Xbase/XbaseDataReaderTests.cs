using System;
using System.Data.Common;
using System.Threading.Tasks;

using FluentAssertions;

using NSubstitute;

using Pilipala.Data.Xbase;
using Pilipala.Tests.Extensions;

using Xunit;

namespace Pilipala.Data.UnitTests.Xbase
{
    public class XbaseDatareaderTests
    {
        private const string _noDataHasBeenReadExceptionMessage =
            "No data has been read. You must advance the cursor past the beginning of the file by calling Read() or ReadAsync() before inspecting the data.";

        private readonly IXbaseDataParser _parser;

        private readonly DbDataReader _reader;

        public XbaseDatareaderTests()
        {
            _parser = Substitute.For<IXbaseDataParser>();
            _reader = new XbaseDataReader(_parser);
        }

        [Fact]
        public void CloseShouldBeCalledWhenDisposing()
        {
            // Act
            _reader.Dispose();

            // Assert
            _parser.Received().Close();
        }

        [Fact]
        public void DepthShouldBeZeroAfterRead()
        {
            // Act
            _reader.Read();

            // Assert
            _reader.Depth.Should().Be(0);
        }

        [Fact]
        public async void DepthShouldBeZeroAfterReadAsync()
        {
            // Act
            await _reader.ReadAsync();

            // Assert
            _reader.Depth.Should().Be(0);
        }

        [Fact]
        public void DepthShouldThrowInvalidOperationExceptionIfNoDataHasBeenRead()
        {
            // Act
            Action depth = () => _reader.Depth.IgnoreUnusedVariable();

            // Assert
            depth
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(_noDataHasBeenReadExceptionMessage);
        }

        [Fact]
        public void IsClosedShouldBeFalseBeforereaderIsClosed()
        {
            // Assert
            _reader.IsClosed.Should().BeFalse();
        }

        [Fact]
        public void IsClosedShouldBeTrueAfterreaderIsClosed()
        {
            // Act
            _reader.Close();

            // Assert
            _reader.IsClosed.Should().BeTrue();
        }

        [Fact]
        public void NextResultAsyncShouldThrowNotImplementedException()
        {
            // Act
            Func<Task<bool>> nextResultAsync = _reader.NextResultAsync;

            // Assert
            nextResultAsync.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void NextResultShouldThrowNotImplementedException()
        {
            // Act
            Action nextResult = () => _reader.NextResult();

            // Assert
            nextResult.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public async void ReadAsyncShouldReturnFalseWhenThereAreNoMoreRecordsToRead()
        {
            // Arrange
            _parser.ReadAsync().Returns(Task.FromResult(true), Task.FromResult(true), Task.FromResult(false));

            // Act
            var firstRead = await _reader.ReadAsync();
            var secondRead = await _reader.ReadAsync();
            var thirdRead = await _reader.ReadAsync();

            // Assert
            firstRead.Should().BeTrue();
            secondRead.Should().BeTrue();
            thirdRead.Should().BeFalse();
            _parser.Received(3).ReadAsync().IgnoreAwaitWarning();
        }

        [Fact]
        public async void ReadAsyncShouldReturnTrueIfThereAreRecordsInTheDatabase()
        {
            // Arrange
            _parser.ReadAsync().Returns(Task.FromResult(true));

            // Act
            var result = await _reader.ReadAsync();

            // Assert
            result.Should().BeTrue();
            _parser.Received().ReadAsync().IgnoreAwaitWarning();
        }

        [Fact]
        public void ReadShouldReturnFalseWhenThereAreNoMoreRecordsToRead()
        {
            // Arrange
            _parser.Read().Returns(true, true, false);

            // Act
            var firstRead = _reader.Read();
            var secondRead = _reader.Read();
            var thirdRead = _reader.Read();

            // Assert
            firstRead.Should().BeTrue();
            secondRead.Should().BeTrue();
            thirdRead.Should().BeFalse();
            _parser.Received(3).Read();
        }

        [Fact]
        public void ReadShouldReturnTrueIfThereAreRecordsInTheDatabase()
        {
            // Arrange
            _parser.Read().Returns(true);

            // Act
            var result = _reader.Read();

            // Assert
            result.Should().BeTrue();
            _parser.Received().Read();
        }

        [Fact]
        public void RecordsAffectedShouldBeZeroAfterreaderIsClosed()
        {
            // Act
            _reader.Close();

            // Assert
            _reader.RecordsAffected.Should().Be(0);
        }

        [Fact]
        public void RecordsAffectedShouldReturnRecordCountAfterRead()
        {
            // Arrange
            _parser.RecordsAffected.Returns(3);

            // Act
            _reader.Read();

            // Assert
            _reader.RecordsAffected.Should().Be(3);
            _parser.Received().RecordsAffected.IgnoreUnusedVariable();
        }

        [Fact]
        public async void RecordsAffectedShouldReturnRecordCountAfterReadAsync()
        {
            // Arrange
            _parser.RecordsAffected.Returns(3);

            // Act
            await _reader.ReadAsync();

            // Assert
            _reader.RecordsAffected.Should().Be(3);
            _parser.Received().RecordsAffected.IgnoreUnusedVariable();
        }

        [Fact]
        public void RecordsAffectedShouldThrowInvalidOperationExceptionIfNoDataHasBeenRead()
        {
            // Act
            Action recordsAffected = () => _reader.RecordsAffected.IgnoreUnusedVariable();

            // Assert
            recordsAffected
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(_noDataHasBeenReadExceptionMessage);
        }
    }
}
