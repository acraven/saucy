using System;

namespace Saucy
{
   public class MessageLogger : ILogMessages
   {
      public void Log(string message)
      {
         Console.WriteLine(message);
      }

      public void Log(string format, params object[] args)
      {
         Console.WriteLine(format, args);
      }
   }
}
