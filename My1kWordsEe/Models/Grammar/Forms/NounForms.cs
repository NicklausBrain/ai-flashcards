using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{


    public class NounForms
    {
        public PartOfSpeech PartOfSpeech => PartOfSpeech.Noun;
        public required string BaseForm { get; init; }
        public NounForm[] List { get; init; } = Array.Empty<NounForm>();
    }

    public record NounForm
    {
        GrammaticalCase Case { get; init; }
        Number Number { get; init; }
        string Value { get; init; }
    }
}