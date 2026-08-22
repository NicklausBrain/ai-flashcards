using CSharpFunctionalExtensions;

using My1kWordsEe.Feature.Samples;
using My1kWordsEe.Feature.Games;

namespace My1kWordsEe.Feature.Samples
{
    public class GetEtSampleSentencesQuery
    {
        private readonly SamplesStorageClient samplesStorageClient;

        public GetEtSampleSentencesQuery(
            SamplesStorageClient samplesStorageClient)
        {
            this.samplesStorageClient = samplesStorageClient;
        }

        public async Task<Result<SampleSentenceWithMedia[]>> Invoke(EtWord word, uint senseIndex)
        {
            var containerId = new SamplesStorageClient.SamplesContainerId { SenseIndex = senseIndex, Word = word.Value };
            var existingSamples = await this.samplesStorageClient.GetEtSampleData(containerId);
            return existingSamples;
        }
    }
}