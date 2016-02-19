using Newtonsoft.Json.Linq;

namespace Saucy.Providers
{
    public interface IProvider
    {
        void Pull(JObject source, string localPath);
    }
}
