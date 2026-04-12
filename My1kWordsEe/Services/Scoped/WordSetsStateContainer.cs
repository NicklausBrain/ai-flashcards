using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Microsoft.AspNetCore.Components.Authorization;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Scoped
{
    public class WordSetsStateContainer
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly WordSetStorageClient wordSetStorageClient;

        private Maybe<Result<IEnumerable<WordSet>>> wordSets;

        public WordSetsStateContainer(
            AuthenticationStateProvider authenticationStateProvider,
            WordSetStorageClient wordSetStorageClient)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.wordSetStorageClient = wordSetStorageClient;
        }

        public async Task<Result<string>> GetUserIdAsync()
        {
            var authState = await this.authenticationStateProvider.GetAuthenticationStateAsync();
            if (authState?.User?.Identity?.IsAuthenticated != true)
            {
                return Result.Failure<string>(Errors.AuthRequired);
            }

            var idClaim = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                return Result.Failure<string>(Errors.AuthRequired);
            }

            var userId = idClaim.Value.Split("|").Last();
            return Result.Success(userId);
        }

        public async Task<Result<IEnumerable<WordSet>>> GetWordSetsAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && wordSets.HasValue && wordSets.Value.IsSuccess)
            {
                return wordSets.Value;
            }

            var userIdResult = await GetUserIdAsync();
            if (userIdResult.IsFailure)
            {
                return Result.Failure<IEnumerable<WordSet>>(userIdResult.Error);
            }

            var result = await this.wordSetStorageClient.ListWordSets(userIdResult.Value);
            this.wordSets = result;
            return result;
        }

        public async Task<Result<Uri>> SaveWordSetAsync(string name, string wordsRaw)
        {
            var userIdResult = await GetUserIdAsync();
            if (userIdResult.IsFailure) return Result.Failure<Uri>(userIdResult.Error);

            var words = wordsRaw.Split(new[] { ',', '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(w => w.Trim())
                                .Where(w => !string.IsNullOrWhiteSpace(w))
                                .ToList();

            if (!words.Any()) return Result.Failure<Uri>("No words provided");

            var wordSet = new WordSet
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userIdResult.Value,
                Name = name,
                Words = words,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await this.wordSetStorageClient.SaveWordSet(wordSet);
            if (result.IsSuccess)
            {
                await GetWordSetsAsync(forceRefresh: true);
            }
            return result;
        }

        public async Task<Result<bool>> DeleteWordSetAsync(string wordSetId)
        {
            var userIdResult = await GetUserIdAsync();
            if (userIdResult.IsFailure) return Result.Failure<bool>(userIdResult.Error);

            var result = await this.wordSetStorageClient.DeleteWordSet(userIdResult.Value, wordSetId);
            if (result.IsSuccess)
            {
                await GetWordSetsAsync(forceRefresh: true);
            }
            return result;
        }
    }
}
