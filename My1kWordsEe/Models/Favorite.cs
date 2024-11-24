namespace My1kWordsEe.Models
{
    public class Favorites
    {
        public required string UserId { get; init; }

        public required IDictionary<string, SampleWord> Words { get; init; }

        // to be added: Phrases
    }
}
