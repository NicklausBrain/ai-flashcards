using System.Text.Json;

using Azure;
using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;

namespace My1kWordsEe.Services.Db
{
    public partial class AzureStorageClient
    {
        public async Task<Result<Maybe<SampleSentenceWithMedia[]>>> GetEtSampleData(string word)
        {
            var container = await GetEtWordsContainer();

            if (container.IsFailure)
            {
                return Result.Failure<Maybe<SampleSentenceWithMedia[]>>(container.Error);
            }

            BlobClient blob = container.Value.GetBlobClient(JsonBlobName(word));

            if (!await blob.ExistsAsync())
            {
                return Maybe<SampleSentenceWithMedia[]>.None;
            }

            try
            {
                var response = await blob.DownloadContentAsync();
                if (response != null && response.HasValue)
                {
                    var samples = JsonSerializer.Deserialize<SampleSentenceWithMedia[]>(response.Value.Content);
                    return Maybe<SampleSentenceWithMedia[]>.From(samples);
                }
                else
                {
                    return Maybe<SampleSentenceWithMedia[]>.None;
                }
            }
            catch (RequestFailedException ex)
            {
                this.logger.LogError(ex, "Failure to download data from blob {name}", blob.Name);
                return Result.Failure<Maybe<SampleSentenceWithMedia[]>>("Failure to download data from blob");
            }
            catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
            {
                this.logger.LogError(ex, "Failure to parse JSON from blob {name}", blob.Name);
                return Result.Failure<Maybe<SampleSentenceWithMedia[]>>("Failure to parse JSON data from blob");
            }
        }

        public Task<Result<Uri>> SaveEtSamplesData(string word, SampleSentenceWithMedia[] samples) =>
            this.GetEtSamplesContainer().Bind(container =>
            this.UploadStreamAsync(
                container.GetBlobClient(JsonBlobName(word)),
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(samples))));

        private Task<Result<BlobContainerClient>> GetEtSamplesContainer() => this.GetOrCreateContainer("et-samples");
    }
}