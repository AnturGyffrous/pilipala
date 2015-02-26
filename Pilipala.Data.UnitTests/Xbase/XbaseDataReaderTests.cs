using FluentAssertions;

using NSubstitute;

using Pilipala.Data.Xbase;
using Pilipala.Tests.AutoFixture;

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
        public void IsClosedShouldBeTrueAfterReaderIsClosed([Frozen] IXbaseDataParser parser, XbaseDataReader reader)
        {
            reader.Close();
            reader.IsClosed.Should().BeTrue();
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
