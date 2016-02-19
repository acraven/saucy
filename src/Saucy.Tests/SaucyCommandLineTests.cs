using System;
using FakeItEasy;
using NUnit.Framework;
using Saucy.Actions;

namespace Saucy.Tests
{
   public class SaucyCommandLineTests
   {
      [Test]
      public void RestoreCallsPackagesRestorer()
      {
         var expectedPath = Environment.CurrentDirectory;

         var packagesRestorer = A.Fake<IRestorePackages>();
         var testSubject = new SaucyCommandLine(packagesRestorer);

         testSubject.Restore();

         A.CallTo(() => packagesRestorer.Restore(expectedPath)).MustHaveHappened(Repeated.Exactly.Once);
      }
   }
}
