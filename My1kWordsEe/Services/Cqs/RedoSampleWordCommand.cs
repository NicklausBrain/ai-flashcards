using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class RedoSampleWordCommand
    {
        private readonly AzureStorageClient azureBlobService;
        private readonly AddSampleWordCommand addSampleWordCommand;
        private readonly DeleteSampleSentenceCommand deleteSampleSentenceCommand;

        public RedoSampleWordCommand(
            AzureStorageClient azureBlobService,
            AddSampleWordCommand addSampleWordCommand,
            DeleteSampleSentenceCommand deleteSampleSentenceCommand)
        {
            this.azureBlobService = azureBlobService;
            this.addSampleWordCommand = addSampleWordCommand;
            this.deleteSampleSentenceCommand = deleteSampleSentenceCommand;
        }

        public async Task<Result<SampleWord>> Invoke(string eeWord, string? comment = null)
        {
            if (!eeWord.ValidateWord())
            {
                return Result.Failure<SampleWord>("Not an Estonian word");
            }

            (await azureBlobService.GetWordData(eeWord)).Deconstruct(
                out bool _,
                out bool isBlobAccessFailure,
                out Maybe<SampleWord> existingWordData,
                out string blobAccessError);

            if (isBlobAccessFailure)
            {
                return Result.Failure<SampleWord>(blobAccessError);
            }

            var redoTask = this.addSampleWordCommand.Invoke(eeWord, comment);

            if (existingWordData.HasValue)
            {
                await Parallel.ForEachAsync(existingWordData.Value.Samples, async (sample, ct) =>
                {
                    if (ct.IsCancellationRequested) { return; }
                    await deleteSampleSentenceCommand.Invoke(sample);
                });
            }

            return await redoTask;
        }
    }
}
