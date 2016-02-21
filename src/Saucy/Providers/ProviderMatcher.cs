using Newtonsoft.Json.Linq;
using Saucy.Exceptions;
using Saucy.Providers;
using System.Linq;

namespace Saucy
{
   public class ProviderMatcher : IMatchProvider
   {
      private readonly IProvider[] _providers;

      public ProviderMatcher(params IProvider[] providers)
      {
         _providers = providers.ToArray();
      }

      public IProvider Match(JObject packageLocator)
      {
         var matches = _providers.Where(c => c.IsMatch(packageLocator)).ToArray();

         if (matches.Length > 1)
         {
            throw new AmbiguousPackageLocatorException(packageLocator);
         }

         if (matches.Length == 0)
         {
            return null;
         }

         return matches[0];
      }
   }
}
