using System.ComponentModel;

namespace My1kWordsEe.Models.Games
{
    [Description("Üks sõnaharjutuse üksus: sõna ja seda sisaldav näidislause")]
    public struct WordGrindItemData
    {
        [Description("Eesti keele sõna TÄPSELT samal kujul nagu sisendis (sama kääne, pööre, arv)")]
        public required string Word { get; init; }

        [Description("Lihtne lause, mis sisaldab sõna täpselt samal kujul. Ära kasuta teisi sisendnimekirja sõnu ega nende vorme.")]
        public required TranslatedString Sentence { get; init; }
    }

    [Description("Sõnaharjutuse mängu andmed")]
    public struct WordGrindGameData
    {
        public WordGrindGameData()
        {
            Items = new List<WordGrindItemData>();
        }

        [Description("Sõnakomplekti identifikaator")]
        public required string WordSetId { get; init; }

        [Description("Iga sisendi sõna kohta üks üksus")]
        public required List<WordGrindItemData> Items { get; init; }
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