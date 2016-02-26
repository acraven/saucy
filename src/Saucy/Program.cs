using CommandLineParser;
using Saucy.Actions;
using Saucy.Providers.GitHub;

namespace Saucy
{
   public class Program
   {
      public static int Main(string[] args)
      {
         var settings = new SaucySettings();
         var restoreVerb = new SaucyCommandLine(
            new PackagesRestorer(
               new JsonLoader(),
               new ProviderMatcher(GitHubProvider.Create()),
               new ConsoleWriter(),
               settings),
            settings);

         var runner = new Runner();
         runner.Register(restoreVerb);

         var exitCode = runner.Run(args);
         return exitCode;
      }
   }
}
