using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services.Db
{
    public interface IGameStorageClient
    {
        Task<Result<Maybe<T>>> GetGameData<T>(string gameId) where T : class;
        Task<Result<Uri>> SaveGameData<T>(string gameId, T gameData) where T : class;
    }
}
