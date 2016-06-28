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

         ILogger logger = new VerboseLogger();

         var restoreVerb = new SaucyCommandLine(
            new PackagesRestorer(
               new JsonLoader(),
               new ProviderMatcher(new GitHubProvider(new FolderSync(logger))),
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
