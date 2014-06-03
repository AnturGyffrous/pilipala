using System.Globalization;

using NUnit.Framework;

using Pilipala.Data.Resources;

namespace Pilipala.Data.UnitTests.ResourceTests
{
    [TestFixture]
    public class ErrorMessagesResourceTests
    {
        [Test]
        public void CanInstantiateTheErrorMessagesResource()
        {
            var resource = new ErrorMessages();
            Assert.That(resource, Is.Not.Null);
        }

        [Test]
        public void CanSetTheCultureOfTheErrorMessagesResource()
        {
            Assert.That(ErrorMessages.Culture, Is.Null);

            var culture = new CultureInfo("en-GB");
            ErrorMessages.Culture = culture;

            Assert.That(ErrorMessages.Culture, Is.EqualTo(culture));

            ErrorMessages.Culture = null;
        }

        [Test]
        public void ErrorMessagesResourceDefaultCultureIsNull()
        {
            Assert.That(ErrorMessages.Culture, Is.Null);
        }
    }
}
