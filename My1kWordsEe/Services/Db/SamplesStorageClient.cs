using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Services.Db
{
    public class SamplesStorageClient
    {
        public struct SamplesContainerId
        {
            public required string Word { get; init; }
            public required uint SenseIndex { get; init; }

            // todo: add user id

            public override string ToString() => $"{Word}-{SenseIndex}";
            public static implicit operator string(SamplesContainerId id) => id.ToString();
        }

        private readonly AzureStorageClient azureStorageClient;

        public SamplesStorageClient(AzureStorageClient azureStorageClient)
        {
            this.azureStorageClient = azureStorageClient;
        }

        public Task<Result<SampleSentenceWithMedia[]>> GetEtSampleData(SamplesContainerId containerId) =>
            this.GetEtSamplesContainer().Bind(container =>
            this.azureStorageClient.DownloadJsonAsync<SampleSentenceWithMedia[]>(
                container.GetBlobClient($"{containerId}.{JsonFormat}"))).Map((samples) =>
                samples.GetValueOrDefault(() => Array.Empty<SampleSentenceWithMedia>()));

        public Task<Result<Uri>> SaveEtSamplesData(
            SamplesContainerId containerId,
            SampleSentenceWithMedia[] samples) =>
            this.GetEtSamplesContainer().Bind(container =>
            this.azureStorageClient.UploadJsonAsync(
                blob: container.GetBlobClient($"{containerId}.{JsonFormat}"),
                record: samples));

        private Task<Result<BlobContainerClient>> GetEtSamplesContainer() => this.azureStorageClient.GetOrCreateContainer("et-samples");
    }
}