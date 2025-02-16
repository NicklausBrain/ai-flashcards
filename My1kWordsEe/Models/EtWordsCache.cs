using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using My1kWordsEe.Models.Semantics;

namespace My1kWordsEe.Models
{
    public class EtWordsCache
    {
        private readonly IWebHostEnvironment environment;
        private readonly Lazy<EtWord[]> _allWords;
        private readonly Lazy<IReadOnlyDictionary<string, string>> _allWordsDiacriticsFree;
        //     private static readonly Dictionary<EtWord, int> WordIndex = Ee1KWords.AllWords
        // .Select((w, i) => (w, i))
        // .ToDictionary(p => p.w, p => p.i + 1);

        public EtWordsCache(IWebHostEnvironment environment)
        {
            this.environment = environment;
            this._allWords = new Lazy<EtWord[]>(LoadEtWords);
            this._allWordsDiacriticsFree = new Lazy<IReadOnlyDictionary<string, string>>(() =>
                AllWords.ToDictionary(w => w.Value, q => RemoveDiacritics(q.Value)));
        }

        public EtWord[] AllWords => _allWords.Value;

        public IReadOnlyDictionary<string, string> AllWordsDiacriticsFree => _allWordsDiacriticsFree.Value;

        private EtWord[] LoadEtWords()
        {
            var dictFile = this.environment.WebRootFileProvider.GetFileInfo("/data/et-words.json");
            var dictStream = dictFile.CreateReadStream();
            var etWords = JsonSerializer.Deserialize<EtWord[]>(dictStream);
            if (etWords is null)
            {
                throw new FileNotFoundException("No /data/et-words.json that is needed for Et vocab");
            }
            return etWords;
        }

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

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}