using System.IO;
using System.Linq;

namespace Saucy
{
   public class FolderSync : ISyncFolders
   {
      private readonly ILogger _logger;

      public FolderSync(ILogger logger)
      {
         _logger = logger;
      }

      public void Sync(string sourcePath, string targetPath)
      {
         _logger.Verbose("Syncing folder {0} with {1}", targetPath, sourcePath);

         if (!Directory.Exists(sourcePath))
         {
            throw new DirectoryNotFoundException(string.Format("{0} does not exist", sourcePath));
         }

         if (!Directory.Exists(targetPath))
         {
            _logger.Verbose("Creating folder {0}", targetPath);
            Directory.CreateDirectory(targetPath);
         }

         SyncFiles(sourcePath, targetPath);
         SyncFolders(sourcePath, targetPath);
      }

      private void SyncFiles(string sourcePath, string targetPath)
      {
         var filesToSync = Directory.GetFiles(sourcePath).Select(Path.GetFileName).ToArray();
         var targetFiles = Directory.GetFiles(targetPath).Select(Path.GetFileName).ToArray();

         var filesToRemove = targetFiles.Where(c => !filesToSync.Contains(c)).ToArray();

         foreach (var file in filesToRemove)
         {
            var fullPath = Path.Combine(targetPath, file);

            _logger.Verbose("Deleting file {0}", fullPath);
            File.Delete(fullPath);
         }

         foreach (var file in filesToSync)
         {
            var sourceFile = Path.Combine(sourcePath, file);
            var targetFile = Path.Combine(targetPath, file);

            // TODO: Any value in comparing contents first before overwriting?
            _logger.Verbose("Syncing file {0}", targetFile);
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
            var fullPath = Path.Combine(targetPath, folder);

            _logger.Verbose("Deleting folder {0}", fullPath);
            Directory.Delete(fullPath, true);
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