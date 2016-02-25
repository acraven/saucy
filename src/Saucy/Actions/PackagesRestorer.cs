using System.IO;
using Newtonsoft.Json.Linq;
using Saucy.Exceptions;

namespace Saucy.Actions
{
   public class PackagesRestorer : IRestorePackages
   {
      private readonly ILoadJson _jsonLoader;
      private readonly IMatchProvider _providerMatcher;
      private readonly ILogMessages _messageLogger;

      public PackagesRestorer(
         ILoadJson jsonLoader,
         IMatchProvider providerMatcher,
         ILogMessages messageLogger)
      {
         _jsonLoader = jsonLoader;
         _providerMatcher = providerMatcher;
         _messageLogger = messageLogger;
      }

      public void Restore(string configPath)
      {
         var config = _jsonLoader.Load(configPath);

         var packages = (JArray)config["packages"];

         var saucyPath = Path.Combine(Path.GetDirectoryName(configPath), "saucy");

         foreach (var packageLocator in packages)
         {
            try
            {
               var provider = _providerMatcher.Match((JObject)packageLocator);

               if (provider != null)
               {
                  provider.Pull((JObject)packageLocator, saucyPath);
               }
               else
               {
                  _messageLogger.Log("Package locator does not match any provider:\r\n{0}", packageLocator.ToString(Newtonsoft.Json.Formatting.None));
               }
            }
            catch (AmbiguousPackageLocatorException)
            {
               _messageLogger.Log("Package locator matches multiple providers:\r\n{0}", packageLocator.ToString(Newtonsoft.Json.Formatting.None));
            }
         }
      }
   }
}