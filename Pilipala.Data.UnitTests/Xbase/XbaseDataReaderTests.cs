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
        [Theory]
        [AutoNSubstituteData]
        public void CloseShouldBeCalledWhenDisposing([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            reader.Dispose();
            parser.Received().Close();
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsClosedShouldBeFalseBeforeReaderIsClosed([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            reader.IsClosed.Should().BeFalse();
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsClosedShouldBeTrueAfterReaderIsClosed([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            reader.Close();
            reader.IsClosed.Should().BeTrue();
        }

        [Theory]
        [AutoNSubstituteData]
        public async void ReadAsyncShouldReturnFalseWhenThereAreNoMoreRecordsToRead([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            parser.ReadAsync().Returns(Task.FromResult(true), Task.FromResult(true), Task.FromResult(false));
            (await reader.ReadAsync()).Should().BeTrue();
            (await reader.ReadAsync()).Should().BeTrue();
            (await reader.ReadAsync()).Should().BeFalse();
            parser.Received(3).ReadAsync().IgnoreAwaitWarning();
        }

        [Theory]
        [AutoNSubstituteData]
        public async void ReadAsyncShouldReturnTrueIfThereAreRecordsInTheDatabase([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            parser.ReadAsync().Returns(Task.FromResult(true));
            (await reader.ReadAsync()).Should().BeTrue();
            parser.Received().ReadAsync().IgnoreAwaitWarning();
        }

        [Theory]
        [AutoNSubstituteData]
        public void ReadShouldReturnFalseWhenThereAreNoMoreRecordsToRead([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            parser.Read().Returns(true, true, false);
            reader.Read().Should().BeTrue();
            reader.Read().Should().BeTrue();
            reader.Read().Should().BeFalse();
            parser.Received(3).Read();
        }

        [Theory]
        [AutoNSubstituteData]
        public void ReadShouldReturnTrueIfThereAreRecordsInTheDatabase([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            parser.Read().Returns(true);
            reader.Read().Should().BeTrue();
            parser.Received().Read();
        }

        [Theory]
        [AutoNSubstituteData]
        public void RecordsAffectedShouldBeZeroAfterReaderIsClosed([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            reader.Close();
            reader.RecordsAffected.Should().Be(0);
        }

        [Fact]
        public void DummyFactToForceNCrunchToRun()
        {
            // See http://forum.ncrunch.net/yaf_postst358_Doesnt-run--Theory--tests.aspx for details
        }
    }
}
