using NUnit.Framework;

using Pilipala.Data.Extensions;

namespace Pilipala.Data.UnitTests.ExtensionTests
{
    [TestFixture]
    public class LinqExtensionTests
    {
        [Test]
        public void CanTakeJustTheFirstTenBytes()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            Assert.That(bytes.Take(10, (byte)0), Is.EqualTo(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }));
        }

        [Test]
        public void CanTakeAllTheBytesUntilTheFirstZeroIsReachedInTheFirstTenBytes()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5, 0, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            Assert.That(bytes.Take(10, (byte)0), Is.EqualTo(new byte[] { 1, 2, 3, 4, 5 }));
        }
    }
}
