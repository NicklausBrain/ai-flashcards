using CSharpFunctionalExtensions;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

using Moq;

namespace My1kWordsEe.Tests.Unit.Feature.Games
{
    // Regression tests for the Estonian leetspeak defect: the AI occasionally echoes
    // Estonian text with digit substitutions (e.g. "värske" -> "ve4rske"). The check
    // commands must return the canonical Estonian/reference fields from local truth,
    // trusting the model only for Match and EnComment.
    public class CheckTranslationLeetTests
    {
        // A dummy connection string keeps TelemetryClient construction offline for tests.
        private readonly TelemetryClient _telemetry = new(new TelemetryConfiguration
        {
            ConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000"
        });

        [Fact]
        public async Task CheckEtTranslation_UsesLocalTruth_WhenAiEchoesLeet()
        {
            var openAiMock = new Mock<OpenAiClient>(null!, null!);
            openAiMock
                .Setup(x => x.CompleteJsonSchemaAsync<EtTranslationCheckResult>(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JsonSchemaRecord>(), It.IsAny<float?>()))
                .ReturnsAsync(Result.Success(new EtTranslationCheckResult
                {
                    EnSentence = "fr3sh f00d",
                    EtUserSentence = "ma ostsin ve4rske toitu poest",
                    EtExpectedSentence = "ma ostsin ve4rsket toitu poest",
                    EnComment = "use the partitive case",
                    Match = 4,
                }));

            var command = new CheckEtTranslationCommand(_telemetry, openAiMock.Object);

            var result = await command.Invoke(
                etSentence: "ma ostsin värske toitu poest",
                enSentence: "I bought fresh food from the store",
                etExpectedSentence: "ma ostsin värsket toitu poest");

            Assert.True(result.IsSuccess);
            Assert.Equal("ma ostsin värske toitu poest", result.Value.EtUserSentence);
            Assert.Equal("ma ostsin värsket toitu poest", result.Value.EtExpectedSentence);
            Assert.Equal("I bought fresh food from the store", result.Value.EnSentence);
            // Match and EnComment are still taken from the model.
            Assert.Equal((ushort)4, result.Value.Match);
            Assert.Equal("use the partitive case", result.Value.EnComment);
        }

        [Fact]
        public async Task CheckEnTranslation_UsesLocalTruth_ForCanonicalFields()
        {
            var openAiMock = new Mock<OpenAiClient>(null!, null!);
            openAiMock
                .Setup(x => x.CompleteJsonSchemaAsync<CheckEnTranslationCommand.Response>(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JsonSchemaRecord>(), It.IsAny<float?>()))
                .ReturnsAsync(Result.Success(new CheckEnTranslationCommand.Response
                {
                    EnComment = "ok",
                    Match = 5,
                }));

            var command = new CheckEnTranslationCommand(_telemetry, openAiMock.Object);

            var result = await command.Invoke(
                etSentence: "väike koer",
                enSentence: "small dog",
                enExpectedSentence: "a small dog");

            Assert.True(result.IsSuccess);
            Assert.Equal("väike koer", result.Value.EeSentence);
            Assert.Equal("small dog", result.Value.EnUserSentence);
            Assert.Equal("a small dog", result.Value.EnExpectedSentence);
            Assert.Equal((ushort)5, result.Value.Match);
            Assert.Equal("ok", result.Value.EnComment);
        }

        [Fact]
        public async Task CheckEeListening_UsesLocalTruth_ForCanonicalFields()
        {
            var openAiMock = new Mock<OpenAiClient>(null!, null!);
            openAiMock
                .Setup(x => x.CompleteJsonSchemaAsync<CheckEeListeningCommand.Response>(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JsonSchemaRecord>(), It.IsAny<float?>()))
                .ReturnsAsync(Result.Success(new CheckEeListeningCommand.Response
                {
                    EnComment = "ok",
                    Match = 5,
                }));

            var command = new CheckEeListeningCommand(_telemetry, openAiMock.Object);

            var result = await command.Invoke(
                etSentence: "väike maja",
                enSentence: "small house",
                userInput: "väike maja");

            Assert.True(result.IsSuccess);
            Assert.Equal("väike maja", result.Value.EeSentence);
            Assert.Equal("small house", result.Value.EnSentence);
            Assert.Equal("väike maja", result.Value.EeUserSentence);
            Assert.Equal((ushort)5, result.Value.Match);
            Assert.Equal("ok", result.Value.EnComment);
        }
    }
}
