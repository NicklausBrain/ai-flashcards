namespace My1kWordsEe.Models
{
    public class Favorites
    {
        public Favorites(
            string userId,
            IDictionary<string, SampleWord>? words = null,
            IDictionary<string, SampleSentence>? sentences = null)
        {
            UserId = userId;
            Words = words ?? new Dictionary<string, SampleWord>();
            Sentences = sentences ?? new Dictionary<string, SampleSentence>();
        }

        public string UserId { get; }

        public IDictionary<string, SampleWord> Words { get; }

        public IDictionary<string, SampleSentence> Sentences { get; }

        public bool IsFavorite(SampleWord word)
        {
            return this.Words.ContainsKey(word.EeWord.ToLowerInvariant());
        }

        public bool IsFavorite(SampleSentence sentence)
        {
            return this.Sentences.ContainsKey(sentence.EeSentence.ToLowerInvariant());
        }
    }
}

