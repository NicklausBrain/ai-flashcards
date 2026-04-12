using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Services.Db
{
    public class WordSetStorageClient
    {
        private const string ContainerId = "word-sets";
        private readonly AzureStorageClient azureStorageClient;

        public WordSetStorageClient(AzureStorageClient azureStorageClient)
        {
            this.azureStorageClient = azureStorageClient;
        }

        public Task<Result<Uri>> SaveWordSet(WordSet wordSet) =>
            this.GetContainer().Bind(container =>
                this.azureStorageClient.UploadJsonAsync(
                    blob: container.GetBlobClient(GetBlobPath(wordSet.UserId, wordSet.Id)),
                    record: wordSet));

        public Task<Result<Maybe<WordSet>>> GetWordSet(string userId, string wordSetId) =>
            this.GetContainer().Bind(container =>
                this.azureStorageClient.DownloadJsonAsync<WordSet>(
                    container.GetBlobClient(GetBlobPath(userId, wordSetId))));

        public async Task<Result<IEnumerable<WordSet>>> ListWordSets(string userId)
        {
            var containerResult = await this.GetContainer();
            if (containerResult.IsFailure)
            {
                return Result.Failure<IEnumerable<WordSet>>(containerResult.Error);
            }

            var container = containerResult.Value;
            var wordSets = new List<WordSet>();
            var prefix = $"{userId}/";

            try
            {
                await foreach (var blobItem in container.GetBlobsAsync(traits: BlobTraits.None, states: BlobStates.None, prefix: prefix, cancellationToken: default))
                {
                    if (blobItem is BlobItem item)
                    {
                        var downloadResult = await this.azureStorageClient.DownloadJsonAsync<WordSet>(
                            container.GetBlobClient(item.Name));
                        if (downloadResult.IsSuccess && downloadResult.Value.HasValue)
                        {
                            wordSets.Add(downloadResult.Value.Value);
                        }
                    }
                }
                return Result.Success<IEnumerable<WordSet>>(wordSets.OrderByDescending(ws => ws.UpdatedAt));
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<WordSet>>($"Failed to list word sets: {ex.Message}");
            }
        }

        public Task<Result<bool>> DeleteWordSet(string userId, string wordSetId) =>
            this.GetContainer().Bind(container =>
                this.azureStorageClient.DeleteIfExistsAsync(
                    container.GetBlobClient(GetBlobPath(userId, wordSetId))));

        private Task<Result<BlobContainerClient>> GetContainer() =>
            this.azureStorageClient.GetOrCreateContainer(ContainerId);

        private static string GetBlobPath(string userId, string wordSetId) =>
            $"{userId}/{wordSetId}.{JsonFormat}";
    }
}
