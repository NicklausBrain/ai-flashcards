using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Moq;
using My1kWordsEe.Models;
using My1kWordsEe.Models.Grammar;
using My1kWordsEe.Models.Grammar.Forms;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services;
using My1kWordsEe.Services.Cqs.Et;
using My1kWordsEe.Services.Db;
using System.Text.Json;
using Xunit;

namespace My1kWordsEe.Tests.Unit.Services.Cqs.Et
{
    public class AddEtFormsCommandTests
    {
        private readonly Mock<OpenAiClient> _openAiClientMock;
        private readonly Mock<FormsStorageClient> _formsStorageClientMock;
        private readonly AddEtFormsCommand _command;

        public AddEtFormsCommandTests()
        {
            _openAiClientMock = new Mock<OpenAiClient>(null!, null!);
            _formsStorageClientMock = new Mock<FormsStorageClient>(null!);
            _command = new AddEtFormsCommand(_openAiClientMock.Object, _formsStorageClientMock.Object);
        }

        [Fact]
        public async Task Invoke_WithAdjective_RequestsAdjectiveFormsFromOpenAi()
        {
            // Arrange
            var word = new EtWord { Value = "suur", Senses = new[] { 
                new WordSense { 
                    Word = new TranslatedString { Et = "suur", En = "big" },
                    Definition = new TranslatedString { Et = "omadussõna", En = "adjective" },
                    BaseForm = "suur",
                    PartOfSpeech = new TranslatedString { Et = "omadussõna", En = "adjective" }
                } 
            } };

            var expectedForms = new AdjectiveForms
            {
                BaseForm = "suur",
                Positive = new AdjectiveDegreeForms { Singular = Array.Empty<NounForm>(), Plural = Array.Empty<NounForm>() },
                Comparative = new AdjectiveDegreeForms { Singular = Array.Empty<NounForm>(), Plural = Array.Empty<NounForm>() },
                Superlative = new AdjectiveDegreeForms { Singular = Array.Empty<NounForm>(), Plural = Array.Empty<NounForm>() }
            };

            _openAiClientMock.Setup(x => x.CompleteJsonSchemaAsync<AdjectiveForms>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<JsonSchemaRecord>(),
                It.IsAny<float?>()))
                .ReturnsAsync(Result.Success(expectedForms));

            _formsStorageClientMock.Setup(x => x.SaveFormsData<AdjectiveForms>(
                It.IsAny<FormsStorageClient.FormsContainerId>(),
                It.IsAny<AdjectiveForms>()))
                .ReturnsAsync(Result.Success(new Uri("http://dummy")));

            // Act
            var result = await _command.Invoke<AdjectiveForms>(word, 0);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("suur", result.Value.BaseForm);
            _openAiClientMock.Verify(x => x.CompleteJsonSchemaAsync<AdjectiveForms>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<JsonSchemaRecord>(),
                It.IsAny<float?>()), Times.Once);
        }

        [Fact]
        public async Task Invoke_WithNoun_RequestsNounFormsFromOpenAi()
        {
            // Arrange
            var word = new EtWord
            {
                Value = "koer",
                Senses = new[] {
                new WordSense {
                    Word = new TranslatedString { Et = "koer", En = "dog" },
                    Definition = new TranslatedString { Et = "loom", En = "animal" },
                    BaseForm = "koer",
                    PartOfSpeech = new TranslatedString { Et = "nimisõna", En = "noun" }
                }
            }
            };

            var expectedForms = new NounForms
            {
                BaseForm = "koer",
                Singular = Array.Empty<NounForm>(),
                Plural = Array.Empty<NounForm>()
            };

            _openAiClientMock.Setup(x => x.CompleteJsonSchemaAsync<NounForms>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<JsonSchemaRecord>(),
                It.IsAny<float?>()))
                .ReturnsAsync(Result.Success(expectedForms));

            _formsStorageClientMock.Setup(x => x.SaveFormsData<NounForms>(
                It.IsAny<FormsStorageClient.FormsContainerId>(),
                It.IsAny<NounForms>()))
                .ReturnsAsync(Result.Success(new Uri("http://dummy")));

            // Act
            var result = await _command.Invoke<NounForms>(word, 0);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("koer", result.Value.BaseForm);
            _openAiClientMock.Verify(x => x.CompleteJsonSchemaAsync<NounForms>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<JsonSchemaRecord>(),
                It.IsAny<float?>()), Times.Once);
        }
    }
}
