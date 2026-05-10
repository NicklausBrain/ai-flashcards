using CSharpFunctionalExtensions;
using Moq;
using My1kWordsEe.Models;
using My1kWordsEe.Models.Grammar;
using My1kWordsEe.Models.Grammar.Forms;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs.Et;
using My1kWordsEe.Services.Db;
using Xunit;

namespace My1kWordsEe.Tests.Unit.Services.Cqs.Et
{
    public class GetOrAddEtFormsCommandTests
    {
        private readonly Mock<FormsStorageClient> _formsStorageClientMock;
        private readonly Mock<AddEtFormsCommand> _addEtFormsCommandMock;
        private readonly GetOrAddEtFormsCommand _command;

        public GetOrAddEtFormsCommandTests()
        {
            _formsStorageClientMock = new Mock<FormsStorageClient>(null!);
            _addEtFormsCommandMock = new Mock<AddEtFormsCommand>(null!, null!);
            _command = new GetOrAddEtFormsCommand(_formsStorageClientMock.Object, _addEtFormsCommandMock.Object);
        }

        private static EtWord CreateWord() => new EtWord
        {
            Value = "suur",
            Senses = new[] {
                new WordSense {
                    BaseForm = "suur",
                    Word = new TranslatedString { Et = "suur", En = "big" },
                    Definition = new TranslatedString { Et = "omadussõna", En = "adjective" },
                    PartOfSpeech = new TranslatedString { Et = "omadussõna", En = "adjective" }
                }
            }
        };

        private static AdjectiveForms CreateForms() => new AdjectiveForms
        {
            BaseForm = "suur",
            Positive = new AdjectiveDegreeForms { Singular = Array.Empty<NounForm>(), Plural = Array.Empty<NounForm>() },
            Comparative = new AdjectiveDegreeForms { Singular = Array.Empty<NounForm>(), Plural = Array.Empty<NounForm>() },
            Superlative = new AdjectiveDegreeForms { Singular = Array.Empty<NounForm>(), Plural = Array.Empty<NounForm>() }
        };

        [Fact]
        public async Task Invoke_WhenFormsExistInStorage_ReturnsFormsFromStorage()
        {
            // Arrange
            var word = CreateWord();
            var expectedForms = CreateForms();

            _formsStorageClientMock.Setup(x => x.GetFormsData<AdjectiveForms>(It.IsAny<FormsStorageClient.FormsContainerId>()))
                .ReturnsAsync(Result.Success(Maybe.From(expectedForms)));

            // Act
            var result = await _command.Invoke<AdjectiveForms>(word, 0);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("suur", result.Value.BaseForm);
            _addEtFormsCommandMock.Verify(x => x.Invoke<AdjectiveForms>(It.IsAny<EtWord>(), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public async Task Invoke_WhenFormsDoNotExistInStorage_CallsAddEtFormsCommand()
        {
            // Arrange
            var word = CreateWord();
            var expectedForms = CreateForms();

            _formsStorageClientMock.Setup(x => x.GetFormsData<AdjectiveForms>(It.IsAny<FormsStorageClient.FormsContainerId>()))
                .ReturnsAsync(Result.Success(Maybe<AdjectiveForms>.None));

            _addEtFormsCommandMock.Setup(x => x.Invoke<AdjectiveForms>(word, 0))
                .ReturnsAsync(Result.Success(expectedForms));

            // Act
            var result = await _command.Invoke<AdjectiveForms>(word, 0);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("suur", result.Value.BaseForm);
            _addEtFormsCommandMock.Verify(x => x.Invoke<AdjectiveForms>(word, 0), Times.Once);
        }
    }
}
