using System.Text.Json;

using Azure;
using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;

namespace My1kWordsEe.Services.Db
{
    public partial class AzureStorageClient
    {
        public async Task<Result<Maybe<SampleWord>>> GetWordData(string word)
        {
            var container = await GetWordsContainer();

            if (container.IsFailure)
            {
                return Result.Failure<Maybe<SampleWord>>(container.Error);
            }

            BlobClient blob = container.Value.GetBlobClient(JsonBlobName(word));

            if (!await blob.ExistsAsync())
            {
                return Maybe<SampleWord>.None;
            }

            try
            {
                var response = await blob.DownloadContentAsync();
                if (response != null && response.HasValue)
                {
                    var sampleWord = JsonSerializer.Deserialize<SampleWord>(response.Value.Content);
                    return Maybe<SampleWord>.From(sampleWord);
                }
                else
                {
                    return Maybe<SampleWord>.None;
                }
            }
            catch (RequestFailedException ex)
            {
                this.logger.LogError(ex, "Failure to download data from blob {name}", blob.Name);
                return Result.Failure<Maybe<SampleWord>>("Failure to download data from blob");
            }
            catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
            {
                this.logger.LogError(ex, "Failure to parse JSON from blob {name}", blob.Name);
                return Result.Failure<Maybe<SampleWord>>("Failure to parse JSON data from blob");
            }
        }

        public Task<Result<Uri>> SaveWordData(SampleWord word) =>
            this.GetWordsContainer().Bind(container =>
            this.UploadStreamAsync(
                container.GetBlobClient(JsonBlobName(word.EeWord)),
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(word))));

        private Task<Result<BlobContainerClient>> GetWordsContainer() => this.GetOrCreateContainer("words");
    }
}

