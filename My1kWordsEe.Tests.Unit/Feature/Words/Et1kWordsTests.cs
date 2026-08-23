using System.Text;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

using Moq;

namespace My1kWordsEe.Tests.Unit.Feature.Words
{
    public class Et1kWordsTests
    {
        [Fact]
        public void WithSearch_WhenSearchMatchesDiacriticsFreeForm_FiltersWords()
        {
            var cache = CreateCacheWithWordsJson("""
                [
                  {
                    "Value": "sõna",
                    "Senses": [
                      {
                        "Word": { "Et": "sõna", "En": "word" },
                        "Definition": { "Et": "keeleüksus", "En": "lexical unit" },
                        "BaseForm": "sõna",
                        "PartOfSpeech": { "Et": "nimisõna", "En": "noun" }
                      }
                    ]
                  },
                  {
                    "Value": "tere",
                    "Senses": [
                      {
                        "Word": { "Et": "tere", "En": "hello" },
                        "Definition": { "Et": "tervitus", "En": "greeting" },
                        "BaseForm": "tere",
                        "PartOfSpeech": { "Et": "hüüdsõna", "En": "interjection" }
                      }
                    ]
                  }
                ]
                """);

            var words = new Et1kWords(cache);

            var result = words.WithSearch("sona", _ => false);

            Assert.Single(result.SelectedWords);
            Assert.Equal("sõna", result.SelectedWords.Single().Value);
        }

        [Fact]
        public void WithSearch_WhenSearchIsInvalid_ReturnsAllNonIgnoredWords()
        {
            var cache = CreateCacheWithWordsJson("""
                [
                  {
                    "Value": "sõna",
                    "Senses": [
                      {
                        "Word": { "Et": "sõna", "En": "word" },
                        "Definition": { "Et": "keeleüksus", "En": "lexical unit" },
                        "BaseForm": "sõna",
                        "PartOfSpeech": { "Et": "nimisõna", "En": "noun" }
                      }
                    ]
                  },
                  {
                    "Value": "tere",
                    "Senses": [
                      {
                        "Word": { "Et": "tere", "En": "hello" },
                        "Definition": { "Et": "tervitus", "En": "greeting" },
                        "BaseForm": "tere",
                        "PartOfSpeech": { "Et": "hüüdsõna", "En": "interjection" }
                      }
                    ]
                  }
                ]
                """);

            var words = new Et1kWords(cache);

            var result = words.WithSearch("??", w => w == "tere");

            Assert.Single(result.SelectedWords);
            Assert.Equal("sõna", result.SelectedWords.Single().Value);
        }

        private static EtWordsCache CreateCacheWithWordsJson(string json)
        {
            var fileInfoMock = new Mock<IFileInfo>();
            fileInfoMock.SetupGet(x => x.Exists).Returns(true);
            fileInfoMock.Setup(x => x.CreateReadStream()).Returns(() => new MemoryStream(Encoding.UTF8.GetBytes(json)));

            var fileProviderMock = new Mock<IFileProvider>();
            fileProviderMock.Setup(x => x.GetFileInfo("/data/et-words.json")).Returns(fileInfoMock.Object);

            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.SetupGet(x => x.WebRootFileProvider).Returns(fileProviderMock.Object);

            var loggerMock = new Mock<ILogger<EtWordsCache>>();

            return new EtWordsCache(environmentMock.Object, loggerMock.Object);
        }
    }
}
