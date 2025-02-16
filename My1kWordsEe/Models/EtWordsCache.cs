using System.Globalization;
using System.Text;
using System.Text.Json;

using My1kWordsEe.Models.Semantics;

namespace My1kWordsEe.Models
{
    public class EtWordsCache
    {
        private readonly IWebHostEnvironment environment;
        private readonly ILogger logger;
        private readonly Lazy<EtWord[]> _allWords;
        private readonly Lazy<IReadOnlyDictionary<string, string>> _allWordsDiacriticsFree;
        private readonly Lazy<IReadOnlyDictionary<EtWord, int>> _wordIndex;

        public EtWordsCache(IWebHostEnvironment environment, ILogger<EtWordsCache> logger)
        {
            this.logger = logger;
            this.environment = environment;
            this._allWords = new Lazy<EtWord[]>(LoadEtWords);
            this._allWordsDiacriticsFree = new Lazy<IReadOnlyDictionary<string, string>>(() =>
                AllWords.ToDictionary(w => w.Value, q => RemoveDiacritics(q.Value)));
            this._wordIndex = new Lazy<IReadOnlyDictionary<EtWord, int>>(() =>
                AllWords.Select((w, i) => (w, i))
                    .ToDictionary(p => p.w, p => p.i + 1));
        }

        public EtWord[] AllWords => _allWords.Value;

        public IReadOnlyDictionary<string, string> AllWordsDiacriticsFree => _allWordsDiacriticsFree.Value;

        public IReadOnlyDictionary<EtWord, int> WordIndex => _wordIndex.Value;

        private EtWord[] LoadEtWords()
        {
            const string dictFilePath = "/data/et-words.json";
            var dictFile = this.environment.WebRootFileProvider.GetFileInfo(dictFilePath);
            if (!dictFile.Exists)
            {
                this.logger.LogError("Dictionary file not found {dictFilePath}", dictFilePath);
                return Array.Empty<EtWord>();
            }
            var dictStream = dictFile.CreateReadStream();
            var etWords = JsonSerializer.Deserialize<EtWord[]>(dictStream);
            if (etWords is null)
            {
                this.logger.LogError("Dictionary file is empty {dictFilePath}", dictFilePath);
                return Array.Empty<EtWord>();
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