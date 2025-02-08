using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class GetOrAddEtWordCommand
    {
        private readonly AzureStorageClient azureBlobService;
        private readonly AddEtWordCommand addEtWordCommand;

        public GetOrAddEtWordCommand(
            AzureStorageClient azureBlobService,
            AddEtWordCommand addEtWordCommand)
        {
            this.azureBlobService = azureBlobService;
            this.addEtWordCommand = addEtWordCommand;
        }

        public async Task<Result<EtWord>> Invoke(string eeWord)
        {
            if (!eeWord.ValidateWord())
            {
                return Result.Failure<EtWord>("Not an Estonian word");
            }

            (await azureBlobService.GetEtWordData(eeWord)).Deconstruct(
                out bool _,
                out bool isBlobAccessFailure,
                out Maybe<EtWord> savedWord,
                out string blobAccessError);

            if (isBlobAccessFailure)
            {
                return Result.Failure<EtWord>(blobAccessError);
            }

            if (savedWord.HasValue)
            {
                return savedWord.Value;
            }

            return await this.addEtWordCommand.Invoke(eeWord);
        }
    }
}