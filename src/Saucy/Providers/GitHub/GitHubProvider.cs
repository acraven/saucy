using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Octokit;

namespace Saucy.Providers.GitHub
{
   public class GitHubProvider : IProvider
   {
      private readonly ISyncFolders _syncFolders;
      private readonly string _accessToken;

      private GitHubProvider(
         ISyncFolders syncFolders,
         string accessToken)
      {
         _syncFolders = syncFolders;
         _accessToken = accessToken;
      }

      public static GitHubProvider Create()
      {
         var accessToken = Environment.GetEnvironmentVariable("GITHUB_ACCESS_TOKEN");

         if (string.IsNullOrEmpty(accessToken))
         {
            throw new InvalidDataException("Environment variable GITHUB_ACCESS_TOKEN not found or empty");
         }

         var provider = new GitHubProvider(new FolderSync(), accessToken);
         return provider;
      }

      public void Pull(JObject packageLocator, string saucyPath)
      {
         var path = packageLocator["path"].ToString();
         var pathElements = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

         var targetPath = Path.Combine(saucyPath, pathElements.Last());
         EnsureFolderExists(targetPath);

         var tmpPath = Path.Combine(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()), pathElements.Last());
         EnsureFolderExists(tmpPath);

         PullToTmpFolder(packageLocator, pathElements, tmpPath);

         _syncFolders.Sync(tmpPath, targetPath);

         Directory.Delete(tmpPath, true);
      }

      private void PullToTmpFolder(JObject packageLocator, string[] pathElements, string cachePath)
      {
         var owner = packageLocator["owner"].ToString();
         var repository = packageLocator["repository"].ToString();
         var commitSha = packageLocator["commit"].ToString();

         var github = new GitHubClient(new ProductHeaderValue("Saucy.Providers.GitHub"))
         {
            Credentials = new Credentials(_accessToken)
         };

         var folderSha = commitSha;

         foreach (var pathElement in pathElements)
         {
            var folderTree = github.Git.Tree.Get(owner, repository, folderSha).Result.Tree;
            var childFolder = folderTree.Single(c => c.Path == pathElement);
            folderSha = childFolder.Sha;
         }

         var sourceTree = github.Git.Tree.GetRecursive(owner, repository, folderSha).Result.Tree;

         foreach (var treeItem in sourceTree)
         {
            if (treeItem.Type == TreeType.Tree)
            {
               EnsureFolderExists(Path.Combine(cachePath, treeItem.Path));
            }
            else if (treeItem.Type == TreeType.Blob)
            {
               var blob = github.Git.Blob.Get(owner, repository, treeItem.Sha).Result;

               WriteFile(Path.Combine(cachePath, treeItem.Path), blob.Content, blob.Encoding);
            }
         }
      }

      private void EnsureFolderExists(string path)
      {
         if (!Directory.Exists(path))
         {
            Directory.CreateDirectory(path);
         }
      }

      private void WriteFile(string path, string content, EncodingType encoding)
      {
         if (encoding == EncodingType.Base64)
         {
            var bytes = Convert.FromBase64String(content);
            var utf8Contents = Encoding.UTF8.GetString(bytes).Replace("\n", Environment.NewLine);

            File.WriteAllText(path, utf8Contents);
         }
         else
         {
            throw new NotSupportedException(string.Format("Encoding of type {0} not supported", encoding));
         }
      }
   }
}
