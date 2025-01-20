namespace My1kWordsEe.Models.Grammar
{
    public class VerbForms
    {
        public PartOfSpeech PartOfSpeech => PartOfSpeech.Verb;

        public required string BaseForm { get; init; }

        public required VerbForm[] List { get; init; } = Array.Empty<VerbForm>();
    }

    public record VerbForm
    {
        public required Pronoun Pronoun { get; init; }

        public required GrammaticalTense Tense { get; init; }

        public required string Value { get; init; } // shall we translate it?
    }
}