using CSharpFunctionalExtensions;

using Moq;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Games;
using My1kWordsEe.Models.Grammar;
using My1kWordsEe.Models.Grammar.Forms;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services;
using My1kWordsEe.Services.Cqs.Et;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Tests.Unit.Models.Games.Generation
{
    public class EtNoun3FormsGameFactoryTest
    {
        private readonly Mock<OpenAiClient> _openAiClientMock;
        private readonly Mock<GetOrAddEtWordCommand> _getOrAddEtWordCommandMock;
        private readonly Mock<GetOrAddEtFormsCommand> _getOrAddEtFormsCommandMock;
        private readonly Mock<GameStorageClient> _gameStorageClientMock;
        private readonly EtNoun3FormsGameFactory _factory;

        public EtNoun3FormsGameFactoryTest()
        {
            _openAiClientMock = new Mock<OpenAiClient>(null!, null!);
            _getOrAddEtWordCommandMock = new Mock<GetOrAddEtWordCommand>(null!, null!);
            _getOrAddEtFormsCommandMock = new Mock<GetOrAddEtFormsCommand>(null!, null!);

            var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configMock.Setup(x => x[AzureStorageClient.ApiSecretKey]).Returns("DefaultEndpointsProtocol=https;AccountName=test;AccountKey=test;EndpointSuffix=core.windows.net");
            var azureStorageClientMock = new Mock<AzureStorageClient>(configMock.Object, new Mock<Microsoft.Extensions.Logging.ILogger<AzureStorageClient>>().Object);
            _gameStorageClientMock = new Mock<GameStorageClient>(azureStorageClientMock.Object);

            _factory = new EtNoun3FormsGameFactory(
                _openAiClientMock.Object,
                _getOrAddEtWordCommandMock.Object,
                _getOrAddEtFormsCommandMock.Object,
                _gameStorageClientMock.Object);
        }

        [Fact]
        public async Task Generate_ShouldReturnCachedGame_WhenCacheHit()
        {
            // Arrange
            var etNoun = "koer";
            var cachedData = new EtNoun3FormsGameFactory.EtNoun3FormsGameData
            {
                NimetavSõna = new TranslatedString { Et = "koer", En = "dog" },
                NimetavLause = new TranslatedString { Et = "See on koer.", En = "This is a dog." },
                OmastavSõna = new TranslatedString { Et = "koera", En = "dog's" },
                OmastavLause = new TranslatedString { Et = "See on koera toit.", En = "This is dog's food." },
                OsastavSõna = new TranslatedString { Et = "koera", En = "dog" },
                OsastavLause = new TranslatedString { Et = "Ma näen koera.", En = "I see a dog." }
            };

            _gameStorageClientMock.Setup(x => x.GetGameData<EtNoun3FormsGameFactory.EtNoun3FormsGameData>(etNoun))
                .ReturnsAsync(Result.Success(Maybe<EtNoun3FormsGameFactory.EtNoun3FormsGameData>.From(cachedData)));

            // Act
            var result = await _factory.Generate(etNoun);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("koer", result.Value.NimetavSõna.Et);
            _openAiClientMock.Verify(x => x.CompleteJsonSchemaAsync<EtNoun3FormsGameFactory.EtNoun3FormsGameData>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JsonSchemaRecord>(), It.IsAny<float?>()), Times.Never);
        }

        [Fact]
        public async Task Generate_ShouldCallOpenAiAndSaveToCache_WhenCacheMiss()
        {
            // Arrange
            var etNoun = "koer";
            var word = new EtWord
            {
                Value = "koer",
                Senses =
                [
                    new WordSense
                    {
                        BaseForm = "koer",
                        PartOfSpeech = new TranslatedString { Et = "nimisõna", En = "noun" },
                        Word = new TranslatedString { Et = "koer", En = "dog" },
                        Definition = new TranslatedString { Et = "koduloom", En = "pet" }
                    }
                ]
            };
            var forms = new NounForms
            {
                BaseForm = "koer",
                Singular = [new NounForm { GrammaticalCase = EtGrammaticalCase.Nimetav, CaseForm = new TranslatedString { Et = "koer", En = "dog" } }],
                Plural = []
            };
            var aiData = new EtNoun3FormsGameFactory.EtNoun3FormsGameData
            {
                NimetavSõna = new TranslatedString { Et = "koer", En = "dog" },
                NimetavLause = new TranslatedString { Et = "See on koer.", En = "This is a dog." },
                OmastavSõna = new TranslatedString { Et = "koera", En = "dog's" },
                OmastavLause = new TranslatedString { Et = "See on koera toit.", En = "This is dog's food." },
                OsastavSõna = new TranslatedString { Et = "koera", En = "dog" },
                OsastavLause = new TranslatedString { Et = "Ma näen koera.", En = "I see a dog." }
            };

            _gameStorageClientMock.Setup(x => x.GetGameData<EtNoun3FormsGameFactory.EtNoun3FormsGameData>(etNoun))
                .ReturnsAsync(Result.Success(Maybe<EtNoun3FormsGameFactory.EtNoun3FormsGameData>.None));

            _getOrAddEtWordCommandMock.Setup(x => x.Invoke(etNoun))
                .ReturnsAsync(Result.Success(word));
            _getOrAddEtFormsCommandMock.Setup(x => x.Invoke<NounForms>(word, 0))
                .ReturnsAsync(Result.Success(forms));

            _openAiClientMock.Setup(x => x.CompleteJsonSchemaAsync<EtNoun3FormsGameFactory.EtNoun3FormsGameData>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JsonSchemaRecord>(), It.IsAny<float?>()))
                .ReturnsAsync(Result.Success(aiData));

            // Act
            var result = await _factory.Generate(etNoun);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("koer", result.Value.NimetavSõna.Et);
            _gameStorageClientMock.Verify(x => x.SaveGameData(etNoun, aiData), Times.Once);
        }
    }
}
