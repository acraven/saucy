using System;
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
      public void Restore()
      {
         _packagesRestorer.Restore(Environment.CurrentDirectory);
      }
   }
}