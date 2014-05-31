using System;
using System.Data.Common;

using NUnit.Framework;

namespace Pilipala.Data.UnitTests
{
    [TestFixture]
    public class DBaseDataReaderTests
    {
        [Test]
        public void DBaseDataReaderDerivesFromTheCorrectType()
        {
            var reader = typeof(DBaseDataReader);
            Assert.That(reader.BaseType, Is.EqualTo(typeof(DbDataReader)));
            Assert.That(reader.GetInterfaces(), Has.Member(typeof(IDisposable)));
        }
    }
}
