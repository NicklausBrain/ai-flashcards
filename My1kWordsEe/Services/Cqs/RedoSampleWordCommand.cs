using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs.Et;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class RedoSampleWordCommand
    {
        private readonly WordStorageClient wordStorageClient;
        private readonly AddEtWordCommand addEtWordCommand;
        private readonly GetEtSampleSentencesQuery getEtSampleSentencesQuery;
        private readonly DeleteEtSampleSentenceCommand deleteEtSampleSentenceCommand;

        public RedoSampleWordCommand(
            WordStorageClient wordStorageClient,
            AddEtWordCommand addEtWordCommand,
            GetEtSampleSentencesQuery getEtSampleSentencesQuery,
            DeleteEtSampleSentenceCommand deleteEtSampleSentenceCommand)
        {
            this.wordStorageClient = wordStorageClient;
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

            (await wordStorageClient.GetEtWordData(etWord)).Deconstruct(
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