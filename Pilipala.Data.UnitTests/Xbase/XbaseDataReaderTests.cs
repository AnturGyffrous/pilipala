using System.Data.Common;

using FluentAssertions;

using NSubstitute;

using Pilipala.Data.Xbase;

using Xunit;

namespace Pilipala.Data.UnitTests.Xbase
{
    public class XbaseDataReaderTests
    {
        [Fact]
        public void CloseShouldBeCalledWhenDisposing()
        {
            var parser = Substitute.For<IXbaseDataParser>();
            DbDataReader reader = new XbaseDataReader(parser);
            reader.Dispose();
            parser.Received().Close();
        }

        [Fact]
        public void IsClosedShouldBeTrueAfterReaderIsClosed()
        {
            var parser = Substitute.For<IXbaseDataParser>();
            DbDataReader reader = new XbaseDataReader(parser);
            reader.Close();
            reader.IsClosed.Should().BeTrue();
        }

        [Fact]
        public void RecordsAffectedShouldBeZeroAfterReaderIsClosed()
        {
            var parser = Substitute.For<IXbaseDataParser>();
            DbDataReader reader = new XbaseDataReader(parser);
            reader.Close();
            reader.RecordsAffected.Should().Be(0);
        }
    }
}
