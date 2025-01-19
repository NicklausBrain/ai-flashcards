using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PartOfSpeech
    {
        None, // Default value | cannot be determined
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