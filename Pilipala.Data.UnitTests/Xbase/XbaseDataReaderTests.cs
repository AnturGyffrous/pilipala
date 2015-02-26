using System;
using System.Threading.Tasks;

using FluentAssertions;

using NSubstitute;

using Pilipala.Data.Xbase;
using Pilipala.Tests.AutoFixture;
using Pilipala.Tests.Extensions;

using Ploeh.AutoFixture.Xunit;

using Xunit;
using Xunit.Extensions;

namespace Pilipala.Data.UnitTests.Xbase
{
    public class XbaseDataReaderTests
    {
        private const string _noDataHasBeenReadExceptionMessage =
            "No data has been read. You must advance the cursor past the beginning of the file by calling Read() or ReadAsync() before inspecting the data.";

        [Theory]
        [AutoNSubstituteData]
        public void CloseShouldBeCalledWhenDisposing([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Act
            reader.Dispose();

            // Assert
            parser.Received().Close();
        }

        [Theory]
        [AutoNSubstituteData]
        public void DepthShouldBeZeroAfterRead([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Act
            reader.Read();

            // Assert
            reader.Depth.Should().Be(0);
        }

        [Theory]
        [AutoNSubstituteData]
        public async void DepthShouldBeZeroAfterReadAsync([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Act
            await reader.ReadAsync();

            // Assert
            reader.Depth.Should().Be(0);
        }

        [Theory]
        [AutoNSubstituteData]
        public void DepthShouldThrowInvalidOperationExceptionIfNoDataHasBeenRead([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Act
            Action depth = () => reader.Depth.IgnoreUnusedVariable();

            // Assert
            depth
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(_noDataHasBeenReadExceptionMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsClosedShouldBeFalseBeforeReaderIsClosed([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Assert
            reader.IsClosed.Should().BeFalse();
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsClosedShouldBeTrueAfterReaderIsClosed([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Act
            reader.Close();

            // Assert
            reader.IsClosed.Should().BeTrue();
        }

        [Theory]
        [AutoNSubstituteData]
        public void NextResultAsyncShouldThrowNotImplementedException([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Act
            Func<Task<bool>> nextResultAsync = reader.NextResultAsync;

            // Assert
            nextResultAsync.ShouldThrow<NotImplementedException>();
        }

        [Theory]
        [AutoNSubstituteData]
        public void NextResultShouldThrowNotImplementedException([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Act
            Action nextResult = () => reader.NextResult();

            // Assert
            nextResult.ShouldThrow<NotImplementedException>();
        }

        [Theory]
        [AutoNSubstituteData]
        public async void ReadAsyncShouldReturnFalseWhenThereAreNoMoreRecordsToRead([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
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

        [Theory]
        [AutoNSubstituteData]
        public async void ReadAsyncShouldReturnTrueIfThereAreRecordsInTheDatabase([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Arrange
            parser.ReadAsync().Returns(Task.FromResult(true));

            // Act
            var result = await reader.ReadAsync();

            // Assert
            result.Should().BeTrue();
            parser.Received().ReadAsync().IgnoreAwaitWarning();
        }

        [Theory]
        [AutoNSubstituteData]
        public void ReadShouldReturnFalseWhenThereAreNoMoreRecordsToRead([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
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

        [Theory]
        [AutoNSubstituteData]
        public void ReadShouldReturnTrueIfThereAreRecordsInTheDatabase([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Arrange
            parser.Read().Returns(true);

            // Act
            var result = reader.Read();

            // Assert
            result.Should().BeTrue();
            parser.Received().Read();
        }

        [Theory]
        [AutoNSubstituteData]
        public void RecordsAffectedShouldBeZeroAfterReaderIsClosed([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Act
            reader.Close();

            // Assert
            reader.RecordsAffected.Should().Be(0);
        }

        [Theory]
        [AutoNSubstituteData]
        public void RecordsAffectedShouldReturnRecordCountAfterRead([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Arrange
            parser.RecordsAffected.Returns(3);

            // Act
            reader.Read();

            // Assert
            reader.RecordsAffected.Should().Be(3);
            parser.Received().RecordsAffected.IgnoreUnusedVariable();
        }

        [Theory]
        [AutoNSubstituteData]
        public async void RecordsAffectedShouldReturnRecordCountAfterReadAsync([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Arrange
            parser.RecordsAffected.Returns(3);

            // Act
            await reader.ReadAsync();

            // Assert
            reader.RecordsAffected.Should().Be(3);
            parser.Received().RecordsAffected.IgnoreUnusedVariable();
        }

        [Theory]
        [AutoNSubstituteData]
        public void RecordsAffectedShouldThrowInvalidOperationExceptionIfNoDataHasBeenRead([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            // Act
            Action recordsAffected = () => reader.RecordsAffected.IgnoreUnusedVariable();

            // Assert
            recordsAffected
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(_noDataHasBeenReadExceptionMessage);
        }

        [Fact]
        public void DummyFactToForceNCrunchToRun()
        {
            // See http://forum.ncrunch.net/yaf_postst358_Doesnt-run--Theory--tests.aspx for details
        }
    }
}
