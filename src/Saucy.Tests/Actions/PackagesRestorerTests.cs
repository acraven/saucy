using System.Collections.Generic;
using System.Linq;
using CommandLineParser;
using FakeItEasy;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Saucy.Actions;
using Saucy.Exceptions;
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
         SetupJsonLoader(@"myFolder\config.json", "{packages:[{package:\"myPackageA\"},{package:\"myPackageB\"}]}");
         SetupProviderMatcher();

         var packageLocatorA = (JObject)_saucyConfig["packages"][0];
         var packageLocatorB = (JObject)_saucyConfig["packages"][1];

         var testSubject = new PackagesRestorer(_jsonLoader, _providerMatcher, A.Fake<IWriteToConsole>(), new SaucySettings());
         testSubject.Restore(@"myFolder\config.json");

         Assert.That(_providerMatcher.MatchPackageLocatorsArgs.ToArray(), Is.EqualTo(new[] { packageLocatorA, packageLocatorB }));
      }

      [Test]
      public void ShouldCallProviderPullForEachPackage()
      {
         var providerA = A.Fake<IProvider>();
         var providerB = A.Fake<IProvider>();

         SetupJsonLoader(@"project\myFolder\config.json", "{packages:[{package:\"myPackageA\"},{package:\"myPackageB\"}]}");
         SetupProviderMatcher(providerA, providerB);

         var packageLocatorA = (JObject)_saucyConfig["packages"][0];
         var packageLocatorB = (JObject)_saucyConfig["packages"][1];

         var testSubject = new PackagesRestorer(_jsonLoader, _providerMatcher, A.Fake<IWriteToConsole>(), new SaucySettings { PackagesFolder = "packagesFolder" });
         testSubject.Restore(@"project\myFolder\config.json");

         A.CallTo(() => providerA.Pull(packageLocatorA, @"project\myFolder\packagesFolder")).MustHaveHappened(Repeated.Exactly.Once);
         A.CallTo(() => providerB.Pull(packageLocatorB, @"project\myFolder\packagesFolder")).MustHaveHappened(Repeated.Exactly.Once);
      }

      [Test]
      public void ShouldLogMessageIfNoMatch()
      {
         SetupJsonLoader(@"project\myFolder\config.json", "{packages:[{\"locator\":\"id\"}]}");
         SetupProviderMatcher();
         var messageLogger = new StubConsoleWriter();

         var testSubject = new PackagesRestorer(_jsonLoader, _providerMatcher, messageLogger, new SaucySettings());

         testSubject.Restore(@"project\myFolder\config.json");

         messageLogger.AssertWrittenMessages(
            @"Restoring packages from project\myFolder\config.json",
            "Package locator does not match any provider: {\"locator\":\"id\"}");
      }

      [Test]
      public void ShouldLogMessageIfMatcherThrowsAmbiguousPackageLocatorException()
      {
         SetupJsonLoader(@"project\myFolder\config.json", "{packages:[{\"locator\":\"id\"}]}");
         var messageLogger = new StubConsoleWriter();
         var myProviderMatcher = A.Fake<IMatchProvider>();
         A.CallTo(() => myProviderMatcher.Match(A<JObject>._)).Throws(new AmbiguousPackageLocatorException(new JObject()));
         
         var testSubject = new PackagesRestorer(_jsonLoader, myProviderMatcher, messageLogger, new SaucySettings());

         testSubject.Restore(@"project\myFolder\config.json");

         messageLogger.AssertWrittenMessages(
            @"Restoring packages from project\myFolder\config.json",
            "Package locator matches multiple providers: {\"locator\":\"id\"}");
      }

      private void SetupJsonLoader(string path, string json)
      {
         _saucyConfig = JObject.Parse(json);

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

            return null;
         }
      }

      private class StubConsoleWriter : IWriteToConsole
      {
         private readonly IList<string> _messages = new List<string>();

         public void Write(string message)
         {
            _messages.Add(message);
         }

         public void Write(string format, params object[] args)
         {
            _messages.Add(string.Format(format, args));
         }

         public void AssertWrittenMessages(params string[] expectedMessages)
         {
            CollectionAssert.AreEqual(expectedMessages, _messages.ToArray());
         }
      }
   }
}
