using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Saucy.Actions;
using Saucy.Providers;

namespace Saucy.Tests.Actions
{
   public class PackagesRestorerTests
   {
      private JObject _saucyConfig;
      private ILoadJson _jsonLoader;
      private StubProviderMatcher _providerMatcher;

      [Test]
      public void ShouldAttemptToGetMatchingProviderForEachPackage()
      {
         SetupJsonLoader(@"myFolder\config.json");
         SetupProviderMatcher();

         var packageLocatorA = (JObject)_saucyConfig["packages"][0];
         var packageLocatorB = (JObject)_saucyConfig["packages"][1];

         var testSubject = new PackagesRestorer(_jsonLoader, _providerMatcher);
         testSubject.Restore(@"myFolder\config.json");

         Assert.That(_providerMatcher.MatchPackageLocatorsArgs.ToArray(), Is.EqualTo(new[] { packageLocatorA, packageLocatorB }));
      }

      [Test]
      public void ShouldCallProviderPullForEachPackage()
      {
         var providerA = A.Fake<IProvider>();
         var providerB = A.Fake<IProvider>();

         SetupJsonLoader(@"project\myFolder\config.json");
         SetupProviderMatcher(providerA, providerB);

         var packageLocatorA = (JObject)_saucyConfig["packages"][0];
         var packageLocatorB = (JObject)_saucyConfig["packages"][1];

         var testSubject = new PackagesRestorer(_jsonLoader, _providerMatcher);
         testSubject.Restore(@"project\myFolder\config.json");

         A.CallTo(() => providerA.Pull(packageLocatorA, @"project\myFolder\saucy")).MustHaveHappened(Repeated.Exactly.Once);
         A.CallTo(() => providerB.Pull(packageLocatorB, @"project\myFolder\saucy")).MustHaveHappened(Repeated.Exactly.Once);
      }

      private void SetupJsonLoader(string path)
      {
         _saucyConfig = JObject.Parse("{packages:[{package:\"myPackageA\"},{package:\"myPackageB\"}]}");

         _jsonLoader = A.Fake<ILoadJson>();
         A.CallTo(() => _jsonLoader.Load(path)).Returns(_saucyConfig);
      }

      private void SetupProviderMatcher(params IProvider[] providersToReturn)
      {
         _providerMatcher = new StubProviderMatcher(providersToReturn);
      }

      private class StubProviderMatcher : IMatchProvider
      {
         private readonly Queue<IProvider> _providersToReturn;
         private readonly List<JObject> _matchPackageLocatorsArgs;

         public StubProviderMatcher(params IProvider[] providersToReturn)
         {
            _providersToReturn = new Queue<IProvider>(providersToReturn);
            _matchPackageLocatorsArgs = new List<JObject>();
         }

         public List<JObject> MatchPackageLocatorsArgs => _matchPackageLocatorsArgs;

         public IProvider Match(JObject packageLocator)
         {
            _matchPackageLocatorsArgs.Add(packageLocator);

            if (_providersToReturn.Any())
            {
               var provider = _providersToReturn.Dequeue();
               return provider;
            }

            return A.Fake<IProvider>();
         }
      }
   }
}
