using CSharpFunctionalExtensions;

using Microsoft.AspNetCore.Components.Authorization;

using My1kWordsEe.Data;
using My1kWordsEe.Models;
using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Services.Scoped
{
    internal class FavoritesStateContainer
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly GetFavoritesQuery getFavoritesQuery;
        private readonly AddToFavoritesCommand addToFavoritesCommand;
        private readonly RemoveFromFavoritesCommand removeFromFavoritesCommand;
        private readonly ReorderFavoritesCommand reorderFavoritesCommand;

        private Maybe<ApplicationUser> user;
        private Maybe<Result<Favorites>> favorites;

        public FavoritesStateContainer(
            AuthenticationStateProvider authenticationStateProvider,
            GetFavoritesQuery getFavoritesQuery,
            AddToFavoritesCommand addToFavoritesCommand,
            RemoveFromFavoritesCommand removeFromFavoritesCommand,
            ReorderFavoritesCommand reorderFavoritesCommand)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.getFavoritesQuery = getFavoritesQuery;
            this.addToFavoritesCommand = addToFavoritesCommand;
            this.removeFromFavoritesCommand = removeFromFavoritesCommand;
            this.reorderFavoritesCommand = reorderFavoritesCommand;
        }

        public async Task<Result<Favorites>> GetAsync()
        {
            if (favorites.HasValue && favorites.Value.IsSuccess)
            {
                return this.favorites.Value;
            }

            var authState = await this.authenticationStateProvider.GetAuthenticationStateAsync();

            if (authState == null)
            {
                return Result.Failure<Favorites>(Errors.AuthRequired);
            }

            var idClaim = authState.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);

            if (idClaim == null)
            {
                return Result.Failure<Favorites>(Errors.AuthRequired);
            }

            this.favorites = await this.getFavoritesQuery.Invoke(idClaim.Value);
            return this.favorites.Value;
        }

        public async Task<Result<Favorites>> AddAsync(dynamic favorite)
        {
            var updatedFavorites = await this.addToFavoritesCommand.Invoke(user.Value.Id, favorite);

            if (updatedFavorites.IsSuccess)
            {
                this.favorites = updatedFavorites;
            }

            return updatedFavorites;
        }

        public async Task<Result<Favorites>> RemoveAsync(dynamic favorite)
        {
            var updatedFavorites = await this.removeFromFavoritesCommand.Invoke(user.Value.Id, favorite);

            if (updatedFavorites.IsSuccess)
            {
                this.favorites = updatedFavorites;
            }

            return updatedFavorites;
        }

        public async Task<Result<Favorites>> ReorderAsync(IEnumerable<SampleWord> sampleWords)
        {
            var updatedFavorites = await this.reorderFavoritesCommand.Invoke(user.Value.Id, sampleWords);

            if (updatedFavorites.IsSuccess)
            {
                this.favorites = updatedFavorites;
            }

            return updatedFavorites;
        }
    }
}
