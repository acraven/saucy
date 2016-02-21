using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Saucy.Providers.GitHub;

namespace Saucy.Tests.Providers.GitHub
{
   public class GitHubProviderTests
   {
      [TestCase("{\"owner\":\"theOwner\",\"repository\":\"theRepo\",\"commit\":\"theCommit\",\"path\":\"thePath\"}", true)]
      [TestCase("{\"repository\":\"theRepo\",\"commit\":\"theCommit\",\"path\":\"thePath\"}", false)]
      [TestCase("{\"owner\":\"theOwner\",\"commit\":\"theCommit\",\"path\":\"thePath\"}", false)]
      [TestCase("{\"owner\":\"theOwner\",\"repository\":\"theRepo\",\"path\":\"thePath\"}", false)]
      [TestCase("{\"owner\":\"theOwner\",\"repository\":\"theRepo\",\"commit\":\"theCommit\"}", false)]
      public void IsMatchReturnsExpectedResult(string json, bool expectedResult)
      {
         var packageLocator = JObject.Parse(json);

         var testSubject = GitHubProvider.Create();

         var result = testSubject.IsMatch(packageLocator);

         Assert.That(result, Is.EqualTo(expectedResult));
      }
   }
}
