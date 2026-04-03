using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services.Db
{
    public interface IAzureStorageClient
    {
        Task<Result<BlobContainerClient>> GetOrCreateContainer(string containerId);
        Task<Result<Uri>> UploadStreamAsync(BlobClient blob, Stream stream);
        Task<Result<Uri>> UploadJsonAsync<T>(BlobClient blob, T record);
        Task<Result<Maybe<T>>> DownloadJsonAsync<T>(BlobClient blob);
        Task<Result<bool>> DeleteIfExistsAsync(BlobClient blob);
    }
}
