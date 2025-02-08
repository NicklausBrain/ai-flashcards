using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs.Et;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class RedoSampleWordCommand
    {
        private readonly AzureStorageClient azureBlobService;
        private readonly AddEtWordCommand addEtWordCommand;
        private readonly GetEtSampleSentencesQuery getEtSampleSentencesQuery;
        private readonly DeleteEtSampleSentenceCommand deleteEtSampleSentenceCommand;

        public RedoSampleWordCommand(
            AzureStorageClient azureBlobService,
            AddEtWordCommand addEtWordCommand,
            GetEtSampleSentencesQuery getEtSampleSentencesQuery,
            DeleteEtSampleSentenceCommand deleteEtSampleSentenceCommand)
        {
            this.azureBlobService = azureBlobService;
            this.addEtWordCommand = addEtWordCommand;
            this.getEtSampleSentencesQuery = getEtSampleSentencesQuery;
            this.deleteEtSampleSentenceCommand = deleteEtSampleSentenceCommand;
        }

        public async Task<Result<EtWord>> Invoke(string etWord, string? comment = null)
        {
            throw new NotImplementedException();

            if (!etWord.ValidateWord())
            {
                return Result.Failure<EtWord>("Not an Estonian word");
            }

            (await azureBlobService.GetEtWordData(etWord)).Deconstruct(
                out bool _,
                out bool isBlobAccessFailure,
                out Maybe<EtWord> existingWordData,
                out string blobAccessError);

            if (isBlobAccessFailure)
            {
                return Result.Failure<EtWord>(blobAccessError);
            }

            var redoTask = this.addEtWordCommand.Invoke(etWord, comment);

            if (existingWordData.HasValue)
            {
                //await Parallel.ForEachAsync(existingWordData.Value.Samples, async (sample, ct) =>
                //{
                //    if (ct.IsCancellationRequested) { return; }
                //    await deleteSampleSentenceCommand.Invoke(sample);
                //});
            }

            return await redoTask;
        }
    }
}