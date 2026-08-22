using CSharpFunctionalExtensions;

using My1kWordsEe.Feature.Grammar;
using My1kWordsEe.Feature.Samples;
using My1kWordsEe.Feature.Games;

namespace My1kWordsEe.Feature.Grammar
{
    public class GetOrAddEtFormsCommand
    {
        private readonly FormsStorageClient formsStorageClient;
        private readonly AddEtFormsCommand addEtFormsCommand;

        public GetOrAddEtFormsCommand(
            FormsStorageClient formsStorageClient,
            AddEtFormsCommand addEtFormsCommand)
        {
            this.formsStorageClient = formsStorageClient;
            this.addEtFormsCommand = addEtFormsCommand;
        }

        public virtual async Task<Result<T>> Invoke<T>(EtWord word, uint senseIndex) where T : IGrammarForms
        {
            var sense = word.Senses[senseIndex];
            var containerId = new FormsStorageClient.FormsContainerId { SenseIndex = senseIndex, BaseForm = sense.BaseForm };

            (await formsStorageClient.GetFormsData<T>(containerId)).Deconstruct(
                out bool _,
                out bool isBlobAccessFailure,
                out Maybe<T> savedForms,
                out string blobAccessError);

            if (isBlobAccessFailure)
            {
                return Result.Failure<T>(blobAccessError);
            }

            if (savedForms.HasValue)
            {
                return Result.Success(savedForms.Value);
            }

            var result = await this.addEtFormsCommand.Invoke<T>(word, senseIndex);
            return result;
        }
    }
}