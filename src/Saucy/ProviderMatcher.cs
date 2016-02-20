using Newtonsoft.Json.Linq;
using Saucy.Providers;
using Saucy.Providers.GitHub;

namespace Saucy
{
   public class ProviderMatcher : IMatchProvider
   {
      public IProvider Match(JObject packageLocator)
      {
         // TODO: Flesh this out
         return GitHubProvider.Create();
      }
   }
}
