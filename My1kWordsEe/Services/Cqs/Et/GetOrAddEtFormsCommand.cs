using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Grammar.Forms;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs.Et
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

        public async Task<Result<T>> Invoke<T>(EtWord word, uint senseIndex) where T : IGrammarForms
        {
            var containerId = new FormsStorageClient.FormsContainerId { SenseIndex = senseIndex, Word = word.Value };

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