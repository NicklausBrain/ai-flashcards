using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class AddSampleWordCommand
    {
        private readonly OpenAiClient openAiService;
        private readonly AzureStorageClient azureBlobService;
        private readonly AddAudioCommand addAudioCommand;

        public AddSampleWordCommand(
            OpenAiClient openAiService,
            AzureStorageClient azureBlobService,
            AddAudioCommand createAudioCommand)
        {
            this.azureBlobService = azureBlobService;
            this.openAiService = openAiService;
            this.addAudioCommand = createAudioCommand;
        }

        public async Task<Result<SampleWord>> Invoke(string eeWord)
        {
            if (!eeWord.ValidateWord())
            {
                return Result.Failure<SampleWord>("Not an Estonian word");
            }

            (await openAiService.GetWordMetadata(eeWord)).Deconstruct(
                out bool _,
                out bool isAiFailure,
                out SampleWord sampleWord,
                out string aiError);

            if (isAiFailure)
            {
                return Result.Failure<SampleWord>(aiError);
            }

            (await this.addAudioCommand.Invoke(eeWord)).Deconstruct(
                out bool isAudioSaved,
                out bool _,
                out Uri audioUri);

            sampleWord = isAudioSaved
                ? sampleWord with { EeAudioUrl = audioUri }
                : sampleWord;

            return (await azureBlobService.SaveWordData(sampleWord))
                .Bind(_ => Result.Of(sampleWord));
        }
    }
}
