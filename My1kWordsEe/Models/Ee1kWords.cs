using System.Globalization;
using System.Text;
using System.Text.Json;

namespace My1kWordsEe.Models
{
    /// <summary>
    /// Represents a collection of 1k most common Estonian words.
    /// </summary>
    public class Ee1kWords
    {
        public Ee1kWords()
        {
            this.Search = null;
            this.SelectedWords = AllWords;
        }

        /// <summary>
        /// Modifies SelectedWords to contain only words that contain the search string.
        /// </summary>
        public Ee1kWords WithSearch(string search)
        {
            if (search.ValidateWord())
            {
                Search = search;
                SelectedWords = AllWords.Where(w =>
                    AllWordsDiacriticsFree[w.EeWord].Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                    w.EnWord.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                    w.EnWords.Any(en => en.Contains(search, StringComparison.InvariantCultureIgnoreCase)) ||
                    w.EeWord.Contains(search, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                Search = search;
                SelectedWords = AllWords;
            }
            return this;
        }

        public Ee1kWords WithSelectedWord(string selectedWord)
        {
            return new Ee1kWords
            {
                Search = this.Search,
                SelectedWords = this.SelectedWords
            };
        }

        public IEnumerable<SampleWord> SelectedWords { get; private set; }

        public string? Search { get; private set; }

        public static readonly SampleWord[] AllWords = Load1kEeWords();

        private static readonly IReadOnlyDictionary<string, string> AllWordsDiacriticsFree =
            AllWords.ToDictionary(w => w.EeWord, q => RemoveDiacritics(q.EeWord));

        private static string RemoveDiacritics(string stIn)
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

        private static SampleWord[] Load1kEeWords()
        {
            var assembly = typeof(SampleWord).Assembly;
            var resourceName = "My1kWordsEe.ee1k.json";
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            return JsonSerializer.Deserialize<SampleWord[]>(stream);
        }
    }
}