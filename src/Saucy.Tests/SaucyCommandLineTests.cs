using System;
using System.IO;
using NUnit.Framework;
using Saucy.Actions;

namespace Saucy.Tests
{
   public class SaucyCommandLineTests
   {
      [Test]
      public void RestoreCallsPackagesRestorerWithDefaultConfig()
      {
         var expectedPath = Path.Combine(Environment.CurrentDirectory, "theConfig.json");

         var packagesRestorer = new StubPackagesRestorer();
         var testSubject = new SaucyCommandLine(packagesRestorer, new SaucySettings { ConfigFile = "theConfig.json" });

         testSubject.Restore(true);

         Assert.That(packagesRestorer.RestoreCallCount, Is.EqualTo(1));
         Assert.That(packagesRestorer.LastRestoreConfigPathArg, Is.EqualTo(expectedPath));
      }

      [Test]
      public void RestoreCallsPackagesRestorerWithExplicitConfig()
      {
         var expectedPath = Path.Combine(Environment.CurrentDirectory, "myConfig.json");

         var packagesRestorer = new StubPackagesRestorer();
         var testSubject = new SaucyCommandLine(packagesRestorer, new SaucySettings());

         testSubject.Restore(true, "myConfig.json");

         Assert.That(packagesRestorer.RestoreCallCount, Is.EqualTo(1));
         Assert.That(packagesRestorer.LastRestoreConfigPathArg, Is.EqualTo(expectedPath));
      }

      [Test]
      public void RestoreCallsPackagesRestorerWithAbsoluteExplicitConfig()
      {
         const string expectedPath = @"C:\myConfig.json";

         var packagesRestorer = new StubPackagesRestorer();
         var testSubject = new SaucyCommandLine(packagesRestorer, new SaucySettings());

         testSubject.Restore(true, @"C:\myConfig.json");

         Assert.That(packagesRestorer.RestoreCallCount, Is.EqualTo(1));
         Assert.That(packagesRestorer.LastRestoreConfigPathArg, Is.EqualTo(expectedPath));
      }

      [Test]
      public void RestoreCallsPackagesRestorerWithDefaultConfigIfPathIsFolder()
      {
         const string expectedPath = @"C:\defaultConfig.json";

         var packagesRestorer = new StubPackagesRestorer();
         var testSubject = new SaucyCommandLine(packagesRestorer, new SaucySettings { ConfigFile = "defaultConfig.json" });

         testSubject.Restore(true, @"C:\");

         Assert.That(packagesRestorer.RestoreCallCount, Is.EqualTo(1));
         Assert.That(packagesRestorer.LastRestoreConfigPathArg, Is.EqualTo(expectedPath));
      }

      private class StubPackagesRestorer : IRestorePackages
      {
         public int RestoreCallCount { get; private set; }

         public string LastRestoreConfigPathArg { get; private set; }

         public void Restore(string configPath)
         {
            RestoreCallCount++;
            LastRestoreConfigPathArg = configPath;
         }
      }
   }
}
