
using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Pronoun
    {
        // Personal Pronouns
        I,          // mina
        YouSingular, // sina
        He,         // tema (ta)
        She,        // tema (ta)
        It,         // see (ta)
        We,         // meie
        YouPlural,  // teie
        They,       // nemad (nad)

        // Possessive Pronouns
        My,         // minu
        YourSingular, // sinu
        His,        // tema (ta)
        Her,        // tema (ta)
        Its,        // selle
        Our,        // meie
        YourPlural, // teie
        Their       // nende
    }
}