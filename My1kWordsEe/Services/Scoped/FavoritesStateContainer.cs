using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Services.Scoped
{
    public class FavoritesStateContainer
    {
        private readonly GetFavoritesQuery getFavoritesQuery;
        private readonly AddToFavoritesCommand addToFavoritesCommand;
        private readonly RemoveFromFavoritesCommand removeFromFavoritesCommand;
        private readonly ReorderFavoritesCommand reorderFavoritesCommand;

        private Maybe<Result<Favorites>> result;

        public FavoritesStateContainer(
            GetFavoritesQuery getFavoritesQuery,
            AddToFavoritesCommand addToFavoritesCommand,
            RemoveFromFavoritesCommand removeFromFavoritesCommand,
            ReorderFavoritesCommand reorderFavoritesCommand)
        {
            this.getFavoritesQuery = getFavoritesQuery;
            this.addToFavoritesCommand = addToFavoritesCommand;
            this.removeFromFavoritesCommand = removeFromFavoritesCommand;
            this.reorderFavoritesCommand = reorderFavoritesCommand;
        }


        public Task<Result<Favorites>> GetAsync()
        {
            if (result.HasValue)
            {
                return this.result;
            }

            this.result = this.getFavoritesQuery.Invoke();

            return this.result;
        }
    }
}
