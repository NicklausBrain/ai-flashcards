using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using Azure;
using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;

namespace My1kWordsEe.Services.Db
{
    public partial class AzureStorageClient
    {
        public struct SamplesContainerId
        {
            public required string Word { get; init; }
            public required uint SenseIndex { get; init; }

            // todo: add user id

            public override string ToString() => $"{Word}-{SenseIndex}";
            public static implicit operator string(SamplesContainerId id) => id.ToString();
        }

        public async Task<Result<SampleSentenceWithMedia[]>> GetEtSampleData(SamplesContainerId containerId)
        {
            var container = await GetEtSamplesContainer();

            if (container.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>(container.Error);
            }

            BlobClient blob = container.Value.GetBlobClient(JsonBlobName(containerId));

            if (!await blob.ExistsAsync())
            {
                return Array.Empty<SampleSentenceWithMedia>();
            }

            try
            {
                var response = await blob.DownloadContentAsync();
                if (response != null && response.HasValue)
                {
                    var samples = JsonSerializer.Deserialize<SampleSentenceWithMedia[]>(response.Value.Content);
                    return samples ?? Array.Empty<SampleSentenceWithMedia>();
                }
                else
                {
                    return Array.Empty<SampleSentenceWithMedia>();
                }
            }
            catch (RequestFailedException ex)
            {
                this.logger.LogError(ex, "Failure to download data from blob {name}", blob.Name);
                return Result.Failure<SampleSentenceWithMedia[]>("Failure to download data from blob");
            }
            catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
            {
                this.logger.LogError(ex, "Failure to parse JSON from blob {name}", blob.Name);
                return Result.Failure<SampleSentenceWithMedia[]>("Failure to parse JSON data from blob");
            }
        }

        public Task<Result<Uri>> SaveEtSamplesData(SamplesContainerId containerId, SampleSentenceWithMedia[] samples) =>
            this.GetEtSamplesContainer().Bind(container =>
            this.UploadStreamAsync(
                container.GetBlobClient(JsonBlobName(containerId)),
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(samples, options: new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = false
                }))));

        private Task<Result<BlobContainerClient>> GetEtSamplesContainer() => this.GetOrCreateContainer("et-samples");
    }
}