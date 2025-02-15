using System.ComponentModel;
using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    /// <summary>
    /// Grammatical cases in Estonian language
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Description("Kääne")]
    public enum GrammaticalCase
    {
        None, // Default value | cannot be determined
        Nominative, // Nimetav
        Genitive,   // Omastav
        Partitive,  // Osastav
        Illative,   // Sisseütlev
        Inessive,   // Seesütlev
        Elative,    // Seestütlev
        Allative,   // Alaleütlev
        Adessive,   // Alalütlev
        Ablative,   // Alaltütlev
        Translative, // Saav
        Terminative, // Rajav
        Essive,     // Olev
        Abessive,   // Ilmaütlev
        Comitative  // Kaasaütlev
    }
}