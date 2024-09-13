using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace My1kWordsEe.Models
{
    public class Ee1kWords
    {
        //private static readonly JsonSerializerOptions options = new JsonSerializerOptions();

        static Ee1kWords()
        {
            //options.Converters.Add(new JsonStringEnumConverter());
            //options.PropertyNameCaseInsensitive = true;
        }

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


        public static readonly EeWord[] AllWords = JsonSerializer.Deserialize<EeWord[]>(
        """
        [
        {
            "value": "ja",
            "part_of_speech": "Sidesõna",
            "parts_of_speech": [
                "Sidesõna"
            ],
            "en_word": "and",
            "en_words": [
                "and"
            ]
        },
        {
            "value": "on",
            "part_of_speech": "Tegusõna",
            "parts_of_speech": [
                "Tegusõna"
            ],
            "en_word": "is",
            "en_words": [
                "is"
            ]
        },
        {
            "value": "ei",
            "part_of_speech": "Määrsõna",
            "parts_of_speech": [
                "Määrsõna"
            ],
            "en_word": "no",
            "en_words": [
                "no",
                "not"
            ]
        },
        {
            "value": "et",
            "part_of_speech": "Sidesõna",
            "parts_of_speech": [
                "Sidesõna"
            ],
            "en_word": "that",
            "en_words": [
                "that"
            ]
        },
        {
            "value": "kui",
            "part_of_speech": "Sidesõna",
            "parts_of_speech": [
                "Sidesõna"
            ],
            "en_word": "if",
            "en_words": [
                "if",
                "when"
            ]
        },
        {
            "value": "oli",
            "part_of_speech": "Tegusõna",
            "parts_of_speech": [
                "Tegusõna"
            ],
            "en_word": "was",
            "en_words": [
                "was"
            ]
        },
        {
            "value": "see",
            "part_of_speech": "Asesõna",
            "parts_of_speech": [
                "Asesõna"
            ],
            "en_word": "this",
            "en_words": [
                "this",
                "it"
            ]
        },
        {
            "value": "mis",
            "part_of_speech": "Asesõna",
            "parts_of_speech": [
                "Asesõna"
            ],
            "en_word": "what",
            "en_words": [
                "what",
                "which"
            ]
        },
        {
            "value": "kes",
            "part_of_speech": "Asesõna",
            "parts_of_speech": [
                "Asesõna"
            ],
            "en_word": "who",
            "en_words": [
                "who"
            ]
        },
        {
            "value": "mina",
            "part_of_speech": "Asesõna",
            "parts_of_speech": [
                "Asesõna"
            ],
            "en_word": "I",
            "en_words": [
                "I"
            ]
        },
        {
            "value": "kaks",
            "part_of_speech": "Arvsõna",
            "parts_of_speech": [
                "Arvsõna"
            ],
            "en_word": "two",
            "en_words": [
                "two"
            ]
        }
        ]
        """);

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
    }
}
