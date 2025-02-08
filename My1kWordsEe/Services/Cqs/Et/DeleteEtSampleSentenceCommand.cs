using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;

using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class DeleteEtSampleSentenceCommand
    {
        private readonly AzureStorageClient azureBlobService;

        public DeleteEtSampleSentenceCommand(
            AzureStorageClient azureBlobService)
        {
            this.azureBlobService = azureBlobService;
        }

        public async Task<Result<SampleSentenceWithMedia[]>> Invoke(
            EtWord word,
            uint senseIndex,
            SampleSentenceWithMedia sampleToRemove)
        {
            var imageRemoval = this.azureBlobService.DeleteImage(sampleToRemove.ImageUrl.Segments.Last());
            var audioRemoval = this.azureBlobService.DeleteAudio(sampleToRemove.AudioUrl.Segments.Last());

            await Task.WhenAll(imageRemoval, audioRemoval);

            if (imageRemoval.Result.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Image removal failed: {imageRemoval.Result.Error}");
            }

            if (audioRemoval.Result.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Speech removal failed: {audioRemoval.Result.Error}");
            }

            var containerId = new AzureStorageClient.SamplesContainerId
            {
                Word = word.Value,
                SenseIndex = senseIndex
            };

            var existingSamples = await this.azureBlobService.GetEtSampleData(containerId);

            if (existingSamples.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>("Sample word not found");
            }

            // check if its ok
            var updatedSamples = existingSamples.Value.Where(s => s.GetHashCode() != sampleToRemove.GetHashCode()).ToArray();

            return (await this.azureBlobService
                .SaveEtSamplesData(containerId, updatedSamples))
                .Bind(r => Result.Success(updatedSamples));
        }
    }
}