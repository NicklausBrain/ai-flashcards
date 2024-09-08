using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class EnsureWordCommand
    {
        private readonly AzureBlobService azureBlobService;
        private readonly OpenAiService openAiService;

        public EnsureWordCommand(
            AzureBlobService azureBlobService,
            OpenAiService openAiService)
        {
            this.azureBlobService = azureBlobService;
            this.openAiService = openAiService;
        }

        public async Task<Result<SampleWord>> Invoke(string eeWord)
        {
            var existingRecord = await azureBlobService.GetWordData(eeWord);

            if (existingRecord.IsSuccess)
            {
                return existingRecord;
            }

            var sampleWord = await openAiService.GetWordMetadata(eeWord);

            if (sampleWord.IsSuccess)
            {
                await azureBlobService.SaveWordData(sampleWord.Value);
            }

            return sampleWord;
        }
    }
}