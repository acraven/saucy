using System;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Saucy.Tests
{
   public class JsonLoaderTests
   {
      [Test]
      public void MissingFileThrowsFileNotFoundException()
      {
         var testSubject = new JsonLoader();
         
         Assert.That(() => testSubject.Load("ThisDoesNotExist.json"), Throws.InstanceOf<FileNotFoundException>());
      }

      [Test]
      public void EmptyFileThrowsJsonReaderException()
      {
         var testSubject = new JsonLoader();

         Assert.That(() => testSubject.Load(@"TestData\EmptyJsonFile.json"), Throws.InstanceOf<JsonReaderException>());
      }
   }
}
