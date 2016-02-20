using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saucy
{
   public class JsonLoader : ILoadJson
   {
      public JObject Load(string path)
      {
         using (var textReader = File.OpenText(path))
         using (var jsonReader = new JsonTextReader(textReader))
         {
            var jobject = JObject.Load(jsonReader);
            return jobject;
         }
      }
   }
}
