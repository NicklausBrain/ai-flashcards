using System.Text.Json;

using Azure;
using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;

namespace My1kWordsEe.Services.Db
{
    public partial class AzureStorageClient
    {
        public async Task<Result<Maybe<SampleSentence[]>>> GetEtSampleData(string word)
        {
            var container = await GetEtWordsContainer();

            if (container.IsFailure)
            {
                return Result.Failure<Maybe<SampleSentence[]>>(container.Error);
            }

            BlobClient blob = container.Value.GetBlobClient(JsonBlobName(word));

            if (!await blob.ExistsAsync())
            {
                return Maybe<SampleSentence[]>.None;
            }

            try
            {
                var response = await blob.DownloadContentAsync();
                if (response != null && response.HasValue)
                {
                    var samples = JsonSerializer.Deserialize<SampleSentence[]>(response.Value.Content);
                    return Maybe<SampleSentence[]>.From(samples);
                }
                else
                {
                    return Maybe<SampleSentence[]>.None;
                }
            }
            catch (RequestFailedException ex)
            {
                this.logger.LogError(ex, "Failure to download data from blob {name}", blob.Name);
                return Result.Failure<Maybe<SampleSentence[]>>("Failure to download data from blob");
            }
            catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
            {
                this.logger.LogError(ex, "Failure to parse JSON from blob {name}", blob.Name);
                return Result.Failure<Maybe<SampleSentence[]>>("Failure to parse JSON data from blob");
            }
        }

        public Task<Result<Uri>> SaveEtSamplesData(string word, SampleSentence[] samples) =>
            this.GetEtSamplesContainer().Bind(container =>
            this.UploadStreamAsync(
                container.GetBlobClient(JsonBlobName(word)),
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(samples))));

        private Task<Result<BlobContainerClient>> GetEtSamplesContainer() => this.GetOrCreateContainer("et_samples");
    }
}