using Newtonsoft.Json.Linq;
using System;

namespace Saucy.Exceptions
{
   public class AmbiguousPackageLocatorException : Exception
   {
      public AmbiguousPackageLocatorException(JObject packageLocator)
         : base(string.Format("Package locator matches multiple providers:\r\n{0}", packageLocator.ToString(Newtonsoft.Json.Formatting.None)))
      {
      }
   }
}
