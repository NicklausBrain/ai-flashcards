using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    /// <summary>
    /// Grammatical cases in Estonian language
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GrammaticalCase
    {
        None, // Default value | cannot be determined
        Nominative,
        Genitive,
        Partitive,
        Illative,
        Inessive,
        Elative,
        Allative,
        Adessive,
        Ablative,
        Translative,
        Terminative,
        Essive,
        Abessive,
        Comitative
    }
}