using System.ComponentModel;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// This is an array wrapper for <see cref="WordSense"/> array
    /// since chat GPT does not support array response.
    /// </summary>
    [Description("Eesti sõnataju")]
    public struct WordSenses
    {
        [Description("Antud sõna")]
        public required string EtWord { get; init; }

        [Description("Sõnatähenduste hulk. Kui antud sõna pole olemas, peaks väljundmassiiv Array olema tühi.")]
        public required WordSense[] Senses { get; init; }
    }
}