using System;
using System.IO;
using CLAP;
using Saucy.Actions;

namespace Saucy
{
   public class SaucyCommandLine
   {
      private readonly IRestorePackages _packagesRestorer;

      public SaucyCommandLine(IRestorePackages packagesRestorer)
      {
         _packagesRestorer = packagesRestorer;
      }

      [Verb(Description = "Restore source code packages to the local filesystem")]
      public void Restore(string configPath = null)
      {
         string rootedConfigPath;

         if (string.IsNullOrEmpty(configPath))
         {
            rootedConfigPath = Path.Combine(Environment.CurrentDirectory, "saucy.json");
         }
         else if (Path.IsPathRooted(configPath))
         {
            rootedConfigPath = configPath;
         }
         else
         {
            rootedConfigPath = Path.Combine(Environment.CurrentDirectory, configPath);
         }

         _packagesRestorer.Restore(rootedConfigPath);
      }
   }
}