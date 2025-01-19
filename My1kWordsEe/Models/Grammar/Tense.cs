using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Tense
    {
        None, // Default value | cannot be determined
        Present,
        Past
    }
}