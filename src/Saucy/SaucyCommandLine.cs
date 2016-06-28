using System;
using System.IO;
using CommandLineParser;
using Saucy.Actions;

namespace Saucy
{
   public class SaucyCommandLine
   {
      private readonly IRestorePackages _packagesRestorer;
      private readonly SaucySettings _settings;

      public SaucyCommandLine(
         IRestorePackages packagesRestorer,
         SaucySettings settings)
      {
         _packagesRestorer = packagesRestorer;
         _settings = settings;
      }

      // Restore source code packages to the local filesystem
      [Verb]
      public void Restore(bool verbose, string configPath = null)
      {
         string rootedConfigPath;

         if (string.IsNullOrEmpty(configPath))
         {
            rootedConfigPath = Path.Combine(Environment.CurrentDirectory, _settings.ConfigFile);
         }
         else
         {
            if (Path.IsPathRooted(configPath))
            {
               rootedConfigPath = configPath;
            }
            else
            {
               rootedConfigPath = Path.Combine(Environment.CurrentDirectory, configPath);
            }

            if (Directory.Exists(configPath))
            {
               rootedConfigPath = Path.Combine(rootedConfigPath, _settings.ConfigFile);
            }
         }

         _packagesRestorer.Restore(rootedConfigPath);
      }
   }
}