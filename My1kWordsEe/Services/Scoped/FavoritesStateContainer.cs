using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components.Authorization;
using My1kWordsEe.Components.Account;
using My1kWordsEe.Data;
using My1kWordsEe.Models;
using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Services.Scoped
{
    internal class FavoritesStateContainer
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IdentityUserAccessor userAccessor;
        private readonly GetFavoritesQuery getFavoritesQuery;
        private readonly AddToFavoritesCommand addToFavoritesCommand;
        private readonly RemoveFromFavoritesCommand removeFromFavoritesCommand;
        private readonly ReorderFavoritesCommand reorderFavoritesCommand;

        private Maybe<ApplicationUser> user;
        private Maybe<Result<Favorites>> favorites;

        public FavoritesStateContainer(
            AuthenticationStateProvider authenticationStateProvider,
            IdentityUserAccessor userAccessor,
            GetFavoritesQuery getFavoritesQuery,
            AddToFavoritesCommand addToFavoritesCommand,
            RemoveFromFavoritesCommand removeFromFavoritesCommand,
            ReorderFavoritesCommand reorderFavoritesCommand)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.userAccessor = userAccessor;
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
                return Result.Failure<Favorites>("User is not authenticated");
            }

            var appUser = await this.userAccessor.GetUserAsync(authState.User);
            if (appUser is null)
            {
                return Result.Failure<Favorites>("User is not authenticated");
            }
            this.user = appUser;
            this.favorites = await this.getFavoritesQuery.Invoke(user.Value.Id);
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
