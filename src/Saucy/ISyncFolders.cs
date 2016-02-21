namespace Saucy
{
   public interface ISyncFolders
   {
      /// <summary>
      /// Ensure contents of sourcePath are copied to targetPath without
      /// unnecessarily deleting contents of targetPath
      /// </summary>
      void Sync(string sourcePath, string targetPath);
   }
}
