using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

using static My1kWordsEe.Services.Db.AzureStorageClient;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class GetEtSampleSentencesQuery
    {

        private readonly AzureStorageClient azureBlobClient;

        public GetEtSampleSentencesQuery(
            AzureStorageClient azureBlobService)
        {
            this.azureBlobClient = azureBlobService;
        }

        public async Task<Result<SampleSentenceWithMedia[]>> Invoke(EtWord word, uint senseIndex)
        {
            var containerId = new SamplesContainerId { SenseIndex = senseIndex, Word = word.Value };
            var existingSamples = await this.azureBlobClient.GetEtSampleData(containerId);
            return existingSamples;
        }
    }
}
