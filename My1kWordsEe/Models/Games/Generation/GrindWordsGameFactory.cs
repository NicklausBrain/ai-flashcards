using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using My1kWordsEe.Services;
using My1kWordsEe.Services.Db;
using My1kWordsEe.Models.Games;

namespace My1kWordsEe.Models.Games.Generation
{
    public class GrindWordsGameFactory
    {
        private readonly OpenAiClient openAiClient;
        private readonly GameStorageClient gameStorageClient;
        private const string GamePrefix = "GrindWordsGame";

        public GrindWordsGameFactory(OpenAiClient openAiClient, GameStorageClient gameStorageClient)
        {
            this.openAiClient = openAiClient;
            this.gameStorageClient = gameStorageClient;
        }

        public async Task<Result<GrindWordsGame>> Generate(IEnumerable<string> words)
        {
            var wordList = words.Select(w => w.Trim().ToLower()).Where(w => !string.IsNullOrWhiteSpace(w)).ToList();
            if (!wordList.Any())
                return Result.Failure<GrindWordsGame>("No words provided");

            var gameId = $"{GamePrefix}-{string.Join("-", wordList)}";
            var cachedGame = await this.gameStorageClient.GetGameData<GrindWordsGame>(gameId);
            if (cachedGame.IsSuccess && cachedGame.Value.HasValue)
                return Result.Success(cachedGame.Value.Value);

            var items = new List<GrindWordItem>();
            foreach (var word in wordList)
            {
                var prompt = $"Generate a simple Estonian sentence (A1-A2 level) using the word '{word}' in context. Also provide an English translation. Return JSON: {{ \"sentence\": string, \"translation\": string }}.";
                var aiResult = await openAiClient.CompleteAsync(prompt, "");
                if (!aiResult.IsSuccess)
                    return Result.Failure<GrindWordsGame>($"AI failed for word '{word}': {aiResult.Error}");
                try
                {
                    var doc = JsonDocument.Parse(aiResult.Value);
                    var sentence = doc.RootElement.GetProperty("sentence").GetString() ?? string.Empty;
                    var translation = doc.RootElement.GetProperty("translation").GetString() ?? string.Empty;
                    var sentenceWithBlank = sentence.Replace(word, "____", StringComparison.OrdinalIgnoreCase);
                    items.Add(new GrindWordItem
                    {
                        OriginalWord = word,
                        SentenceWithBlank = sentenceWithBlank,
                        CorrectAnswer = word,
                        Translation = translation
                    });
                }
                catch (Exception ex)
                {
                    return Result.Failure<GrindWordsGame>($"Failed to parse AI response for '{word}': {ex.Message}");
                }
            }
            var game = new GrindWordsGame { Items = items };
            await this.gameStorageClient.SaveGameData(gameId, game);
            return Result.Success(game);
        }
    }
}
