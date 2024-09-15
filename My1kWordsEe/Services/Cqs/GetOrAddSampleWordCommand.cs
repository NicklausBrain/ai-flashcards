using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class GetOrAddSampleWordCommand
    {
        private readonly AzureBlobService azureBlobService;
        private readonly AddSampleWordCommand addSampleWordCommand;

        public GetOrAddSampleWordCommand(
            AzureBlobService azureBlobService,
            AddSampleWordCommand addSampleWordCommand)
        {
            this.azureBlobService = azureBlobService;
            this.addSampleWordCommand = addSampleWordCommand;
        }

        public async Task<Result<SampleWord>> Invoke(string eeWord)
        {
            (await azureBlobService.GetWordData(eeWord)).Deconstruct(
                out bool _,
                out bool isBlobAccessFailure,
                out Maybe<SampleWord> savedWord,
                out string blobAccessError);

            if (isBlobAccessFailure)
            {
                return Result.Failure<SampleWord>(blobAccessError);
            }

            if (savedWord.HasValue)
            {
                return savedWord.Value;
            }

            return await this.addSampleWordCommand.Invoke(eeWord);
        }
    }
}