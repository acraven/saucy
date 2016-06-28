namespace Saucy
{
   using System;

   public class VerboseLogger : ILogger
   {
      public void Verbose(string format, params object[] args)
      {
         Console.WriteLine(format, args);
      }
   }
}
