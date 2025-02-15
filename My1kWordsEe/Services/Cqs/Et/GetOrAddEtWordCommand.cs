using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class GetOrAddEtWordCommand
    {
        private readonly WordStorageClient wordStorageClient;
        private readonly AddEtWordCommand addEtWordCommand;

        public GetOrAddEtWordCommand(
            WordStorageClient wordStorageClient,
            AddEtWordCommand addEtWordCommand)
        {
            this.wordStorageClient = wordStorageClient;
            this.addEtWordCommand = addEtWordCommand;
        }

        public async Task<Result<EtWord>> Invoke(string eeWord)
        {
            if (!eeWord.ValidateWord())
            {
                return Result.Failure<EtWord>("Not an Estonian word");
            }

            (await wordStorageClient.GetEtWordData(eeWord)).Deconstruct(
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