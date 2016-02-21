using FakeItEasy;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Saucy.Exceptions;
using Saucy.Providers;

namespace Saucy.Tests.Providers
{
   public class ProviderMatcherTests
   {
      private IProvider _providerA;
      private IProvider _providerB;
      private JObject _packageLocator;

      [SetUp]
      public void SetupBeforeEachTest()
      {
         _providerA = A.Fake<IProvider>();
         _providerB = A.Fake<IProvider>();
         _packageLocator = new JObject();
      }

      [Test]
      public void MatchesFirstProvider()
      {
         A.CallTo(() => _providerA.IsMatch(_packageLocator)).Returns(true);
         A.CallTo(() => _providerB.IsMatch(_packageLocator)).Returns(false);

         var testSubject = new ProviderMatcher(_providerA, _providerB);

         var result = testSubject.Match(_packageLocator);

         Assert.That(result, Is.SameAs(_providerA));
      }

      [Test]
      public void MatchesSecondProvider()
      {
         A.CallTo(() => _providerA.IsMatch(_packageLocator)).Returns(false);
         A.CallTo(() => _providerB.IsMatch(_packageLocator)).Returns(true);

         var testSubject = new ProviderMatcher(_providerA, _providerB);

         var result = testSubject.Match(_packageLocator);

         Assert.That(result, Is.SameAs(_providerB));
      }

      [Test]
      public void ReturnsNullIfNoMatch()
      {
         A.CallTo(() => _providerA.IsMatch(_packageLocator)).Returns(false);
         A.CallTo(() => _providerB.IsMatch(_packageLocator)).Returns(false);

         var testSubject = new ProviderMatcher(_providerA, _providerB);

         var result = testSubject.Match(_packageLocator);

         Assert.That(result, Is.Null);
      }

      [Test]
      public void ReturnsNullIfNoProviders()
      {
         var testSubject = new ProviderMatcher();

         var result = testSubject.Match(_packageLocator);

         Assert.That(result, Is.Null);
      }

      [Test]
      public void ThrowExceptionIfMultipleProvidersMatch()
      {
         A.CallTo(() => _providerA.IsMatch(_packageLocator)).Returns(true);
         A.CallTo(() => _providerB.IsMatch(_packageLocator)).Returns(true);

         var testSubject = new ProviderMatcher(_providerA, _providerB);

         Assert.That(() => testSubject.Match(_packageLocator), Throws.InstanceOf<AmbiguousPackageLocatorException>());
      }
   }
}
