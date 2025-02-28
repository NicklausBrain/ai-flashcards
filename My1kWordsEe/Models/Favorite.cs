using My1kWordsEe.Models.Semantics;

namespace My1kWordsEe.Models
{
    public record Favorites
    {
        private const int MinWordScore = 0;
        private const int MaxWordScore = 10;

        public string UserId { get; init; } = string.Empty;

        public IDictionary<string, EtWord> Words { get; init; } = new Dictionary<string, EtWord>();

        public IDictionary<string, SampleSentenceWithMedia> Sentences { get; init; } = new Dictionary<string, SampleSentenceWithMedia>();

        public IDictionary<string, int> Stats { get; init; } = new Dictionary<string, int>();

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