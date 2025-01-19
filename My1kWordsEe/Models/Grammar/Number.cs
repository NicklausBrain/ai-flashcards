using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Number
    {
        None, // Default value | cannot be determined
        Singular,
        Plural
    }
}