using Microsoft.Playwright;

using My1kWordsEe.Tests.E2E.Infra;

namespace My1kWordsEe.Tests.E2E;

[TestFixture]
public class GameRenderingTests : BlazorTest
{
    [Test]
    public async Task TranslateToEnglishGameRendering()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + "translate-game/en");

        // Wait for game to load
        await Page.WaitForSelectorAsync(".spinner-border", new() { State = WaitForSelectorState.Hidden });

        // Verify heading
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Translate to English" })).ToBeVisibleAsync();

        // Verify image is present
        await Expect(Page.Locator("#sampleImage")).ToBeVisibleAsync();

        // Verify Submit and Give up buttons
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Submit" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Give up" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Word2WordMatchGameRendering()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + "word-2-word-match-game/en");

        // Wait for game to load (spinner should disappear)
        await Page.WaitForSelectorAsync(".spinner-border", new() { State = WaitForSelectorState.Hidden });

        // Verify heading
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Match word pairs" })).ToBeVisibleAsync();

        // Verify that there are cards (5 English + 5 Estonian)
        var gameCards = Page.Locator(".card[role='button']");
        await Expect(gameCards).ToHaveCountAsync(10);
    }

    [Test]
    public async Task TranslateToEstonianGameRendering()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + "translate-game/et");

        // Wait for game to load (spinner should disappear)
        await Page.WaitForSelectorAsync(".spinner-border", new() { State = WaitForSelectorState.Hidden });

        // Verify heading renders
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Translate to Estonian" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task ListenToEstonianGameRendering()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + "listen-game/et");

        // Wait for game to load (spinner should disappear)
        await Page.WaitForSelectorAsync(".spinner-border", new() { State = WaitForSelectorState.Hidden });

        // Verify heading renders
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Listen and catch Estonian speech" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Noun3FormsGameRendering()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + "forms-game/et");

        // Wait for game to load (spinner should disappear)
        await Page.WaitForSelectorAsync(".spinner-border", new() { State = WaitForSelectorState.Hidden });

        // Verify heading renders
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Nimisõna 3 vormi (Noun 3 Forms)" })).ToBeVisibleAsync();
    }
}
