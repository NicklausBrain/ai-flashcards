namespace My1kWordsEe.Models
{
    public class FavoriteWord
    {
        public required string EeWord { get; set; }

        public required string EnWord { get; init; }
    }

    public class Favorites
    {
        public required string UserId { get; set; }

        public List<FavoriteWord> Words { get; set; } = new List<FavoriteWord>();

        // to be added: Phrases
    }
}
