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
        public async Task<Result<Maybe<EtWord>>> GetEtWordData(string word)
        {
            var container = await GetEtWordsContainer();

            if (container.IsFailure)
            {
                return Result.Failure<Maybe<EtWord>>(container.Error);
            }

            BlobClient blob = container.Value.GetBlobClient(JsonBlobName(word));

            if (!await blob.ExistsAsync())
            {
                return Maybe<EtWord>.None;
            }

            try
            {
                var response = await blob.DownloadContentAsync();
                if (response != null && response.HasValue)
                {
                    var sampleWord = JsonSerializer.Deserialize<EtWord>(response.Value.Content);
                    return Maybe<EtWord>.From(sampleWord);
                }
                else
                {
                    return Maybe<EtWord>.None;
                }
            }
            catch (RequestFailedException ex)
            {
                this.logger.LogError(ex, "Failure to download data from blob {name}", blob.Name);
                return Result.Failure<Maybe<EtWord>>("Failure to download data from blob");
            }
            catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
            {
                this.logger.LogError(ex, "Failure to parse JSON from blob {name}", blob.Name);
                return Result.Failure<Maybe<EtWord>>("Failure to parse JSON data from blob");
            }
        }

        public Task<Result<Uri>> SaveEtWordData(EtWord word) =>
            this.GetEtWordsContainer().Bind(container =>
            this.UploadStreamAsync(
                container.GetBlobClient(JsonBlobName(word.Value)),
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(word, options: new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = false
                }))));

        private Task<Result<BlobContainerClient>> GetEtWordsContainer() => this.GetOrCreateContainer("et-word");
    }
}