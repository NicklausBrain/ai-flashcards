using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Grammar.Forms;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class GetOrAddEtFormsCommand
    {
        private readonly FormsStorageClient formsStorageClient;
        // private readonly AddEtWordCommand addEtWordCommand;

        public GetOrAddEtFormsCommand(
            FormsStorageClient formsStorageClient
            //AddEtWordCommand addEtWordCommand
            )
        {
            this.formsStorageClient = formsStorageClient;
            //this.addEtWordCommand = addEtWordCommand;
        }

        public async Task<Result<IGrammarForms>> Invoke(EtWord word, uint senseIndex)
        {
            // if (!eeWord.ValidateWord())
            // {
            //     return Result.Failure<IGrammarForms>("Not an Estonian word");
            // }

            var containerId = new FormsStorageClient.FormsContainerId { SenseIndex = senseIndex, Word = word.Value };

            (await formsStorageClient.GetFormsData(containerId)).Deconstruct(
                out bool _,
                out bool isBlobAccessFailure,
                out Maybe<IGrammarForms> savedForms,
                out string blobAccessError);

            if (isBlobAccessFailure)
            {
                return Result.Failure<IGrammarForms>(blobAccessError);
            }

            if (savedForms.HasValue)
            {
                return Result.Success(savedForms.Value);
            }

            return Result.Failure<IGrammarForms>("not implemented");

            //return await this.addEtWordCommand.Invoke(eeWord);
        }
    }
}
