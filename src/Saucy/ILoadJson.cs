using Newtonsoft.Json.Linq;

namespace Saucy
{
   public interface ILoadJson
   {
      JObject Load(string path);
   }
}
