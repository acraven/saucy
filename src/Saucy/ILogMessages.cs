using System;

namespace Saucy
{
   public interface ILogMessages
   {
      void Log(string message);

      void Log(string format, params object[] args);
   }
}
