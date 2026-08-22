using System.Text.Json.Serialization;

namespace My1kWordsEe.Feature.Grammar
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GrammaticalNumber
    {
        None, // Default value | cannot be determined
        Singular,
        Plural
    }
}