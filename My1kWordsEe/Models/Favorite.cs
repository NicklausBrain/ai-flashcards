using My1kWordsEe.Models.Semantics;

namespace My1kWordsEe.Models
{
    public record Favorites
    {
        public const int MinWordScore = 0;
        public const int MaxWordScore = 10;

        public string UserId { get; init; } = string.Empty;

        public IDictionary<string, EtWord> Words { get; init; } = new Dictionary<string, EtWord>();

        public IDictionary<string, SampleSentenceWithMedia> Sentences { get; init; } = new Dictionary<string, SampleSentenceWithMedia>();

        public IDictionary<string, int> Stats { get; init; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public bool IsKnown(EtWord word)
        {
            var wordKey = word.Value.ToLowerInvariant();
            return this.Stats.ContainsKey(wordKey) && this.Stats[wordKey] == MaxWordScore;
        }

        public bool IsKnown(string word)
        {
            var wordKey = word.ToLowerInvariant();
            return this.Stats.ContainsKey(wordKey) && this.Stats[wordKey] == MaxWordScore;
        }

        public bool IsFavorite(EtWord word)
        {
            return this.Words.ContainsKey(word.Value.ToLowerInvariant());
        }

        public bool IsFavorite(ISampleEtSentence sentence)
        {
            return this.Sentences.ContainsKey(sentence.Sentence.Et.ToLowerInvariant());
        }
    }
}