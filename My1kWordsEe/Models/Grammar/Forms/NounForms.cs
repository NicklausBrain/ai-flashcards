namespace My1kWordsEe.Models.Grammar
{
    public class NounForms
    {
        public PartOfSpeech PartOfSpeech => PartOfSpeech.Noun;

        public required string BaseForm { get; init; }

        public required NounForm[] List { get; init; } = Array.Empty<NounForm>();
    }

    public record NounForm
    {
        public required GrammaticalCase Case { get; init; }

        public required GrammaticalNumber Number { get; init; }

        public required Dictionary<LanguageCode, string> Value { get; init; } = new();
    }
}