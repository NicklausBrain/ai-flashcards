using System.Threading.Tasks;
using Microsoft.Playwright;
using My1kWordsEe.Tests.E2E.Infra;
using NUnit.Framework;

namespace My1kWordsEe.Tests.E2E
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class GrindWordsGameE2ETest : BlazorTest
    {
        [Test]
        public async Task GrindWordsGame_FullFlow()
        {
            // Navigate to grind-words game page (assume route is /grind-words-game)
            await Page.GotoAsync(RootUri.AbsoluteUri + "grind-words-game");
            await Page.WaitForSelectorAsync(".spinner-border", new() { State = WaitForSelectorState.Hidden });

            // Verify heading
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Fill in the blank" })).ToBeVisibleAsync();

            // Input answer for first item
            var input = Page.GetByRole(AriaRole.Textbox, new() { Name = "Your answer" });
            await input.FillAsync("testword");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

            // Check feedback (correct/incorrect)
            var feedback = Page.GetByTestId("answer-feedback");
            await Expect(feedback).ToBeVisibleAsync();

            // Next navigation
            var nextBtn = Page.GetByRole(AriaRole.Button, new() { Name = "Next" });
            await nextBtn.ClickAsync();

            // Repeat for second item (if present)
            if (await input.IsVisibleAsync())
            {
                await input.FillAsync("wrongword");
                await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
                await Expect(feedback).ToBeVisibleAsync();
            }

            // Complete game and check for completion message
            var doneMsg = Page.GetByTestId("game-complete-message");
            await Expect(doneMsg).ToBeVisibleAsync();
        }
    }
}
