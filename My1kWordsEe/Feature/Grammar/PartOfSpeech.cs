using System.ComponentModel;
using System.Text.Json.Serialization;

namespace My1kWordsEe.Feature.Grammar
{
    [Description("Grammatical category of the word")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PartOfSpeech
    {
        /// <summary>
        /// Cannot be determined - Default value
        /// </summary>
        [Description("Cannot be determined")]
        None,
        Noun,
        Adjective,
        Pronoun,
        Numeral,
        Verb,
        Adverb,
        Interjection,
        Conjunction,
        Preposition,
        Article,
        Determiner
    }
}