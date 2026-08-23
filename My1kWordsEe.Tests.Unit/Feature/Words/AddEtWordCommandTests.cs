namespace My1kWordsEe.Tests.Unit.Feature.Words
{
    public class AddEtWordCommandTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("two words")]
        [InlineData("@@@")]
        public async Task Invoke_WhenInputIsNotAValidEstonianWord_ReturnsFailure(string input)
        {
            var command = new AddEtWordCommand(openAiClient: null!, wordStorageClient: null!, generateSpeechCommand: null!);

            var result = await command.Invoke(input);

            Assert.True(result.IsFailure);
            Assert.Equal("Not an Estonian word", result.Error);
        }
    }
}
