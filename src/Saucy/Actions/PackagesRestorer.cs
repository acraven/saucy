using System.IO;
using Newtonsoft.Json.Linq;

namespace Saucy.Actions
{
   public class PackagesRestorer : IRestorePackages
   {
      private readonly ILoadJson _jsonLoader;
      private readonly IMatchProvider _providerMatcher;

      public PackagesRestorer(
         ILoadJson jsonLoader,
         IMatchProvider providerMatcher)
      {
         _jsonLoader = jsonLoader;
         _providerMatcher = providerMatcher;
      }

      public void Restore(string configPath)
      {
         var config = _jsonLoader.Load(configPath);

         var packages = (JArray)config["packages"];

         var saucyPath = Path.Combine(Path.GetDirectoryName(configPath), "saucy");

         foreach (var packageLocator in packages)
         {
            var provider = _providerMatcher.Match((JObject)packageLocator);

            provider.Pull((JObject)packageLocator, saucyPath);
         }
      }
   }
}