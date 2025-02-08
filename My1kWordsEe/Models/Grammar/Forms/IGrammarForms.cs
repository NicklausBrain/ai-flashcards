namespace My1kWordsEe.Models.Grammar.Forms
{
    public interface IGrammarForms
    {
        public TranslatedString PartOfSpeech { get; init; }

        public string BaseForm { get; init; }
    }
}
