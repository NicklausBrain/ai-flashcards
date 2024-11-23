namespace My1kWordsEe.Models
{
    public class FavoriteWord
    {
        public required string EeWord { get; init; }

        public required string EnWord { get; init; }
    }

    public class Favorites
    {
        public required string UserId { get; init; }

        public required IList<FavoriteWord> Words { get; init; }

        // to be added: Phrases
    }
}
