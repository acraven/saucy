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
      private readonly SaucySettings _settings;

      public PackagesRestorer(
        ILoadJson jsonLoader,
        IMatchProvider providerMatcher,
        IWriteToConsole consoleWriter,
        SaucySettings settings)
      {
         _jsonLoader = jsonLoader;
         _providerMatcher = providerMatcher;
         _consoleWriter = consoleWriter;
         _settings = settings;
      }

      public void Restore(string configPath)
      {
         var config = _jsonLoader.Load(configPath);

         var packages = (JArray)config["packages"];

         var packagesPath = Path.Combine(Path.GetDirectoryName(configPath), _settings.PackagesFolder);

         foreach (var packageLocator in packages)
         {
            try
            {
               var provider = _providerMatcher.Match((JObject)packageLocator);

               if (provider != null)
               {
                  provider.Pull((JObject)packageLocator, packagesPath);
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