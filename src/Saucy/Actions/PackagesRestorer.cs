using System.IO;
using CommandLineParser;
using Newtonsoft.Json.Linq;
using Saucy.Exceptions;

namespace Saucy.Actions
{
   public class PackagesRestorer : IRestorePackages
   {
      private readonly ILoadJson _jsonLoader;
      private readonly IMatchProvider _providerMatcher;
      private readonly IWriteToConsole _consoleWriter;

      public PackagesRestorer(
         ILoadJson jsonLoader,
         IMatchProvider providerMatcher,
         IWriteToConsole consoleWriter)
      {
         _jsonLoader = jsonLoader;
         _providerMatcher = providerMatcher;
         _consoleWriter = consoleWriter;
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
                  _consoleWriter.Write("Package locator does not match any provider: {0}", packageLocator.ToString(Newtonsoft.Json.Formatting.None));
               }
            }
            catch (AmbiguousPackageLocatorException)
            {
               _consoleWriter.Write("Package locator matches multiple providers: {0}", packageLocator.ToString(Newtonsoft.Json.Formatting.None));
            }
         }
      }
   }
}