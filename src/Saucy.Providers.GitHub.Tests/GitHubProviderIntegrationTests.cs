using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Saucy.Providers.GitHub.Tests
{
    //[Category("integration")]
    public class GitHubProviderIntegrationTests
    {
        private string _localPath;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            _localPath = Guid.NewGuid().ToString();
        }

        [Test]
        public void pull_first_commit_of_example_a()
        {
            Console.WriteLine("Environment.NewLine length {0}", Environment.NewLine.Length);

            var testSubject = new GitHubProvider();
            var source = JObject.Parse("{owner:\"acraven\",repository:\"saucy-examples\",commit:\"39f87ac936ae9fc1b11ef749538e61417d447917\",path:\"src/Saucy.Example.ProjectA\"}");

            testSubject.Pull(source, _localPath);

            AssertFoldersAreEqual(_localPath, @"TestData\FirstCommitProjectA");
        }

        [Test]
        public void pull_second_commit_of_example_a()
        {
            var testSubject = new GitHubProvider();
            var source = JObject.Parse("{owner:\"acraven\",repository:\"saucy-examples\",commit:\"d01f7c95a06a347fc6d73fa8c5fbe121d355ebc2\",path:\"src/Saucy.Example.ProjectA\"}");

            testSubject.Pull(source, _localPath);

            AssertFoldersAreEqual(_localPath, @"TestData\SecondCommitProjectA");
        }

        //todo: truncated
        //todo: missing path element
        //todo: handle invalid responses
        //todo: multiple folder levels / vs \
        //todo: file encoding

        private static void AssertFoldersAreEqual(string actualFolder, string expectedFolder)
        {
            var actualFiles = Directory.GetFiles(actualFolder).Select(Path.GetFileName).ToArray();
            var expectedFiles = Directory.GetFiles(expectedFolder).Select(Path.GetFileName).ToArray();

            Assert.That(actualFiles, Is.EquivalentTo(expectedFiles), string.Format("Names and number of files must match in {0} and {1}", actualFolder, expectedFolder));

            var actualFolders = Directory.GetDirectories(actualFolder).Select(c => new DirectoryInfo(c).Name).ToArray();
            var expectedFolders = Directory.GetDirectories(expectedFolder).Select(c => new DirectoryInfo(c).Name).ToArray();

            Assert.That(actualFolders, Is.EquivalentTo(expectedFolders), string.Format("Names and number of folders must match in {0} and {1}", actualFolder, expectedFolder));

            foreach (var file in expectedFiles)
            {
                var actualFile = Path.Combine(actualFolder, file);
                var expectedFile = Path.Combine(expectedFolder, file);

                var actualContents = File.ReadAllBytes(actualFile);
                var expectedContents = File.ReadAllBytes(expectedFile);

                Assert.That(actualContents.Length, Is.EqualTo(expectedContents.Length), string.Format("Length of files {0} and {1} must match", actualFile, expectedFile));
                Assert.That(actualContents, Is.EquivalentTo(expectedContents), string.Format("Content of files {0} and {1} must match", actualFile, expectedFile));
            }

            foreach (var folder in expectedFolders)
            {
                AssertFoldersAreEqual(Path.Combine(actualFolder, folder), Path.Combine(expectedFolder, folder));
            }
        }
    }
}
