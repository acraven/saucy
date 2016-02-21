using Newtonsoft.Json.Linq;
using Saucy.Providers;

namespace Saucy
{
   public interface IMatchProvider
   {
      IProvider Match(JObject packageLocator);
   }
}
