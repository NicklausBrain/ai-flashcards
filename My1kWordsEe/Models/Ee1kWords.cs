using System.Globalization;
using System.Text;
using System.Text.Json;

namespace My1kWordsEe.Models
{
    public class Ee1kWords
    {
        public Ee1kWords()
        {
            this.SelectedWord = null;
            this.Search = null;
            this.SelectedWords = AllWords;
        }

        public Ee1kWords WithSearch(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return new Ee1kWords
                {
                    SelectedWord = this.SelectedWord,
                    Search = search,
                    SelectedWords = AllWords
                };
            }
            else
            {
                return new Ee1kWords
                {
                    SelectedWord = this.SelectedWord,
                    Search = search,
                    SelectedWords = AllWords
                        .Where(w => w.Value.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                            || _allWordsDiacriticsFree[w.Value].Contains(search, StringComparison.InvariantCultureIgnoreCase))
                        .ToArray()
                };
            }
        }

        public Ee1kWords WithSelectedWord(string selectedWord)
        {
            return new Ee1kWords
            {
                SelectedWord = selectedWord,
                Search = this.Search,
                SelectedWords = this.SelectedWords
            };
        }

        public EeWord[] SelectedWords { get; private set; }

        public string? Search { get; private set; }

        public string? SelectedWord { get; private set; }

        public static readonly EeWord[] AllWords = Load1kEeWords();

        private static readonly IReadOnlyDictionary<string, string> _allWordsDiacriticsFree =
            AllWords.ToDictionary(w => w.Value, q => RemoveDiacritics(q.Value));

        static string RemoveDiacritics(string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        private static EeWord[] Load1kEeWords()
        {
            var assembly = typeof(EeWord).Assembly;
            var resourceName = "My1kWordsEe.ee1k.json";
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            return JsonSerializer.Deserialize<EeWord[]>(stream);
        }
    }
}
