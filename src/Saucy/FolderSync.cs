using System.IO;
using System.Linq;

namespace Saucy
{
   public class FolderSync : ISyncFolders
   {
      public void Sync(string sourcePath, string targetPath)
      {
         if (!Directory.Exists(sourcePath))
         {
            throw new DirectoryNotFoundException(string.Format("{0} does not exist", sourcePath));
         }

         if (!Directory.Exists(targetPath))
         {
            Directory.CreateDirectory(targetPath);
         }

         SyncFiles(sourcePath, targetPath);
         SyncFolders(sourcePath, targetPath);
      }

      private static void SyncFiles(string sourcePath, string targetPath)
      {
         var filesToSync = Directory.GetFiles(sourcePath).Select(Path.GetFileName).ToArray();
         var targetFiles = Directory.GetFiles(targetPath).Select(Path.GetFileName).ToArray();

         var filesToRemove = targetFiles.Where(c => !filesToSync.Contains(c)).ToArray();

         foreach (var file in filesToRemove)
         {
            File.Delete(Path.Combine(targetPath, file));
         }

         foreach (var file in filesToSync)
         {
            var sourceFile = Path.Combine(sourcePath, file);
            var targetFile = Path.Combine(targetPath, file);

            // TODO: Any value in comparing contents first before overwriting?
            File.Copy(sourceFile, targetFile, true);
         }
      }

      private void SyncFolders(string sourcePath, string targetPath)
      {
         var foldersToSync = Directory.GetDirectories(sourcePath).Select(c => new DirectoryInfo(c).Name).ToArray();
         var existingFolders = Directory.GetDirectories(targetPath).Select(c => new DirectoryInfo(c).Name).ToArray();

         var foldersToRemove = existingFolders.Where(c => !foldersToSync.Contains(c)).ToArray();

         foreach (var folder in foldersToRemove)
         {
            Directory.Delete(Path.Combine(targetPath, folder));
         }

         foreach (var folder in foldersToSync)
         {
            var sourceFolder = Path.Combine(sourcePath, folder);
            var targetFolder = Path.Combine(targetPath, folder);

            Sync(sourceFolder, targetFolder);
         }
      }
   }
}