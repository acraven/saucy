using System;

namespace Saucy.Actions
{
   public class PackagesRestorer : IRestorePackages
   {
      public void Restore(string saucyConfigPath)
      {
         Console.WriteLine("Restore!");
      }
   }
}