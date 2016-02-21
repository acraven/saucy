using System;
using CLAP;
using Saucy.Actions;
using Saucy.Providers.GitHub;

namespace Saucy
{
   public class Program
   {
      public static void Main(string[] args)
      {
         var parser = new Parser<SaucyCommandLine>();

         parser.Register.EmptyHandler(() => WriteUsage(parser));
         parser.Register.HelpHandler("?,h,help", Console.WriteLine);
         parser.Register.ErrorHandler(e => { WriteUsage(parser); Console.WriteLine(e.Exception.Message); });

         parser.Run(args, new SaucyCommandLine(new PackagesRestorer(new JsonLoader(), new ProviderMatcher(GitHubProvider.Create()))));
      }

      private static void WriteUsage(MultiParser parser)
      {
         Console.WriteLine("USAGE: saucy");
         Console.WriteLine(parser.GetHelpString());
      }
   }
}
