using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;

using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class DeleteEtSampleSentenceCommand
    {
        private readonly SamplesStorageClient samplesStorageClient;
        private readonly ImageStorageClient imageStorageClient;
        private readonly AudioStorageClient audioStorageClient;

        public DeleteEtSampleSentenceCommand(
            SamplesStorageClient samplesStorageClient,
            ImageStorageClient imageStorageClient,
            AudioStorageClient audioStorageClient)
        {
            this.samplesStorageClient = samplesStorageClient;
            this.imageStorageClient = imageStorageClient;
            this.audioStorageClient = audioStorageClient;
        }

        public async Task<Result<SampleSentenceWithMedia[]>> Invoke(
            EtWord word,
            uint senseIndex,
            SampleSentenceWithMedia sampleToRemove)
        {
            var imageRemoval = this.imageStorageClient.DeleteImage(sampleToRemove.ImageFileName);
            var audioRemoval = this.audioStorageClient.DeleteAudio(sampleToRemove.AudioFileName);
            var promptRemoval = this.imageStorageClient.DeleteImage(sampleToRemove.ImagePromptFileName);

            await Task.WhenAll(imageRemoval, audioRemoval, promptRemoval);

            if (imageRemoval.Result.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Image removal failed: {imageRemoval.Result.Error}");
            }

            if (audioRemoval.Result.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Speech removal failed: {audioRemoval.Result.Error}");
            }

            var containerId = new SamplesStorageClient.SamplesContainerId
            {
                Word = word.Value,
                SenseIndex = senseIndex
            };

            var existingSamples = await this.samplesStorageClient.GetEtSampleData(containerId);

            if (existingSamples.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>("Sample word not found");
            }

            // check if its ok
            var updatedSamples = existingSamples.Value.Where(s => s.GetHashCode() != sampleToRemove.GetHashCode()).ToArray();

            return (await this.samplesStorageClient
                .SaveEtSamplesData(containerId, updatedSamples))
                .Bind(r => Result.Success(updatedSamples));
        }
    }
}