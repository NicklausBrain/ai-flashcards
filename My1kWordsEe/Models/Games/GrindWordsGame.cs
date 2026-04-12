using System.Collections.Generic;

namespace My1kWordsEe.Models.Games
{
    public class GrindWordsGame
    {
        public List<GrindWordItem> Items { get; set; } = new();
        public int CurrentIndex { get; set; } = 0;
        public int Score { get; set; } = 0;
        public bool IsCompleted => CurrentIndex >= Items.Count;
    }

    public class GrindWordItem
    {
        public string OriginalWord { get; set; } = string.Empty;
        public string SentenceWithBlank { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string? UserInput { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
