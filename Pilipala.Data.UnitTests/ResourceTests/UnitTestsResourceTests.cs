using System.Globalization;

using NUnit.Framework;

namespace Pilipala.Data.UnitTests.ResourceTests
{
    [TestFixture]
    public class UnitTestsResourceTests
    {
        [Test]
        public void CanInstantiateTheResource()
        {
            var resource = new Properties.Resources();
            Assert.That(resource, Is.Not.Null);
        }

        [Test]
        public void CanSetTheCultureOfTheErrorMessagesResource()
        {
            Assert.That(Properties.Resources.Culture, Is.Null);

            var culture = new CultureInfo("en-GB");
            Properties.Resources.Culture = culture;

            Assert.That(Properties.Resources.Culture, Is.EqualTo(culture));

            Properties.Resources.Culture = null;
        }

        [Test]
        public void ErrorMessagesResourceDefaultCultureIsNull()
        {
            Assert.That(Properties.Resources.Culture, Is.Null);
        }
    }
}
