using My1kWordsEe.Models.Semantics;

namespace My1kWordsEe.Models
{
    public record Favorites
    {
        private const int MinWordScore = 0;
        private const int MaxWordScore = 10;

        public Favorites(
            string userId,
            IDictionary<string, EtWord>? words = null,
            IDictionary<string, SampleSentenceWithMedia>? sentences = null,
            IDictionary<string, int>? stats = null)
        {
            UserId = userId;
            Words = words ?? new Dictionary<string, EtWord>();
            Sentences = sentences ?? new Dictionary<string, SampleSentenceWithMedia>();
            Stats = stats ?? new Dictionary<string, int>();
        }

        public string UserId { get; }

        public IDictionary<string, EtWord> Words { get; }

        public IDictionary<string, SampleSentenceWithMedia> Sentences { get; }

        public IDictionary<string, int> Stats { get; }

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