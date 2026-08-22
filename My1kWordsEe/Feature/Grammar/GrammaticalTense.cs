using System.Text.Json.Serialization;

namespace My1kWordsEe.Feature.Grammar
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GrammaticalTense
    {
        None, // Default value | cannot be determined
        Present,
        Past
    }
}