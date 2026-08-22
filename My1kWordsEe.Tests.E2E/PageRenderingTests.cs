using System.Text.RegularExpressions;

using Microsoft.Playwright;

using My1kWordsEe.Tests.E2E.Infra;

namespace My1kWordsEe.Tests.E2E;

[TestFixture]
public class PageRenderingTests : BlazorTest
{
    [Test]
    public async Task HomePage()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri, new() { WaitUntil = WaitUntilState.DOMContentLoaded });

        await Expect(Page).ToHaveTitleAsync(new Regex("Words"));
    }

    [TestCase("auto", "[car]")]
    [TestCase("kütus", "[fuel]")]
    public async Task WordPage(string eeWord, string enWord)
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + $"word/{eeWord}", new() { WaitUntil = WaitUntilState.DOMContentLoaded });

        await Expect(Page.GetByText(enWord)).ToBeVisibleAsync();
    }

    [Test]
    public async Task FavoritesPage()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + "favorites", new() { WaitUntil = WaitUntilState.DOMContentLoaded });

        // It should probably show a message that no favorites are found or just the heading
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Favorites" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task WordSetsPage()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + "word-sets", new() { WaitUntil = WaitUntilState.DOMContentLoaded });

        // Verify primary heading renders
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Word Grind | Sõnaharjutused" })).ToBeVisibleAsync();

        // Verify the create-set form landmark is present
        await Expect(Page.GetByText("Create New Word Set")).ToBeVisibleAsync();
    }
}
