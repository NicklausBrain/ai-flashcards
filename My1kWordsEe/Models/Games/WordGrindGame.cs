using System;
using System.Collections.Generic;
using System.Linq;

namespace My1kWordsEe.Models.Games
{
    public record WordGrindItemData
    {
        public required string Word { get; init; }
        public required TranslatedString Sentence { get; init; }
    }

    public record WordGrindGameData
    {
        public required string WordSetId { get; init; }
        public required List<WordGrindItemData> Items { get; init; } = new List<WordGrindItemData>();
    }

    public class WordGrindItem
    {
        public WordGrindItem(WordGrindItemData data)
        {
            this.Word = data.Word;
            this.Sentence = data.Sentence;
        }

        public string Word { get; }
        public TranslatedString Sentence { get; }
        public string UserAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; private set; }

        public void Evaluate()
        {
            this.IsCorrect = string.Equals(this.Word.Trim(), this.UserAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }

    public class WordGrindGame
    {
        public WordGrindGame(WordGrindGameData data)
        {
            this.WordSetId = data.WordSetId;
            this.Items = data.Items.Select(i => new WordGrindItem(i)).ToList();
        }

        public string WordSetId { get; }
        public List<WordGrindItem> Items { get; }
        public bool IsFinished { get; private set; }
        public int Score { get; private set; }

        public void Submit()
        {
            if (this.IsFinished) return;

            foreach (var item in this.Items)
            {
                item.Evaluate();
                if (item.IsCorrect)
                {
                    this.Score++;
                }
            }

            this.IsFinished = true;
        }

        public void GiveUp()
        {
            foreach (var item in this.Items)
            {
                item.Evaluate();
            }
            this.IsFinished = true;
        }

        public bool IsMaxScore => this.Score == this.Items.Count && this.Items.Count > 0;
    }
}
