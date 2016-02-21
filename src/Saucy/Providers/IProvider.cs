using Newtonsoft.Json.Linq;

namespace Saucy.Providers
{
   public interface IProvider
   {
      bool IsMatch(JObject packageLocator);

      void Pull(JObject packageLocator, string saucyPath);
   }
}
