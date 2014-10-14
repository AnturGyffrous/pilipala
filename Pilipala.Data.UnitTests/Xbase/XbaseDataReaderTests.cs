using System.Data.Common;

using FluentAssertions;

using Pilipala.Data.Xbase;

using Xunit;

namespace Pilipala.Data.UnitTests.Xbase
{
    public class XbaseDataReaderTests
    {
        [Fact]
        public void XbaseDataReaderDerivesFromDbDataReader()
        {
            typeof(XbaseDataReader).BaseType.Should().Be<DbDataReader>();
        }
    }
}
