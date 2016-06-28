using CommandLineParser;
using Saucy.Actions;
using Saucy.Providers.GitHub;

namespace Saucy
{
   using System;

   public class Program
   {
      public static int Main(string[] args)
      {
         var consoleWriter = new ConsoleWriter();

         try
         {
            var settings = new SaucySettings();

            ILogger logger = new VerboseLogger();

            var restoreVerb = new SaucyCommandLine(
               new PackagesRestorer(
                  new JsonLoader(),
                  new ProviderMatcher(new GitHubProvider(new FolderSync(logger))),
                  consoleWriter,
                  settings),
               settings);

            var runner = new Runner();
            runner.Register(restoreVerb);

            var exitCode = runner.Run(args);
            return exitCode;
         }
         catch (Exception e)
         {
            consoleWriter.Write(e.Message);
            consoleWriter.Write(e.StackTrace);
            return -1;
         }
      }
   }
}
