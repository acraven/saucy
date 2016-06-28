namespace Saucy
{
   public interface ILogger
   {
      void Verbose(string format, params object[] args);
   }
}
