namespace My1kWordsEe.Models.Grammar
{
    public class AdjectiveForms
    {
        public PartOfSpeech PartOfSpeech => PartOfSpeech.Adjective;

        public required string BaseForm { get; init; }

        public required AdjectiveForm[] List { get; init; } = Array.Empty<AdjectiveForm>();
    }

    public record AdjectiveForm
    {
        public required GrammaticalCase Case { get; init; }

        public required GrammaticalNumber Number { get; init; }

        public required string Value { get; init; } // can we translate it?
    }
}