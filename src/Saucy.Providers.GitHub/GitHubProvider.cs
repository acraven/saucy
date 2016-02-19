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
        public void Pull(JObject source, string localPath)
        {
            var owner = source["owner"].ToString();
            var repository = source["repository"].ToString();
            var commitSha = source["commit"].ToString();
            var path = source["path"].ToString();
            var pathElements = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            var targetPath = Path.Combine(localPath, pathElements.Last());
            EnsureFolderExists(targetPath);

            var github = new GitHubClient(new ProductHeaderValue("Saucy.Providers.GitHub"));

            // TODO: DO NOT COMMIT WITH REAL CREDENTIALS
            //var basicAuth = new Credentials(null, null);
            //github.Credentials = basicAuth;

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
                    EnsureFolderExists(Path.Combine(targetPath, treeItem.Path));
                }
                else if (treeItem.Type == TreeType.Blob)
                {
                    var blob = github.Git.Blob.Get(owner, repository, treeItem.Sha).Result;

                    WriteFile(Path.Combine(targetPath, treeItem.Path), blob.Content, blob.Encoding);
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
