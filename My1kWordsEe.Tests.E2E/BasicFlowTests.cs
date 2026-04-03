using System.Text.RegularExpressions;
using Microsoft.Playwright;
using My1kWordsEe.Tests.E2E.Infra;
using NUnit.Framework;

namespace My1kWordsEe.Tests.E2E;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BasicFlowTests : BlazorTest
{
    [Test]
    public async Task SearchAndNavigateToWord()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri);
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Type "auto" in the search box
        var searchInput = Page.GetByLabel("Search");
        await searchInput.FillAsync("auto");
        
        // Wait for the grid to update - it should contain "auto"
        var autoCell = Page.GetByRole(AriaRole.Cell, new() { Name = "auto", Exact = true }).First;
        await Expect(autoCell).ToBeVisibleAsync();
        
        // Click on the cell with "auto"
        await autoCell.ClickAsync();
        
        // Verify redirected to /word/auto
        await Expect(Page).ToHaveURLAsync(new Regex("/word/auto"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "auto" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task ExercisesNavigation()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri);
        
        // Click on Exercises in NavMenu
        await Page.GetByRole(AriaRole.Link, new() { Name = "Exercises" }).ClickAsync();
        
        // Verify heading
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Exercises / Ülesandeid" })).ToBeVisibleAsync();
        
        // Verify at least one game card is present
        await Expect(Page.GetByText("Translate to English")).ToBeVisibleAsync();
    }

    [Test]
    public async Task WordPageTabs()
    {
        // Go directly to a noun word page
        await Page.GotoAsync(RootUri.AbsoluteUri + "word/auto");
        
        // Verify we are on the word page
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "auto" })).ToBeVisibleAsync();
        
        // Check if Samples tab is active by default (it's the first tab)
        var samplesTab = Page.GetByRole(AriaRole.Tab, new() { Name = "Samples", Exact = true });
        await Expect(samplesTab).ToHaveAttributeAsync("aria-selected", "true");
        
        // Click on Forms tab
        var formsTab = Page.GetByRole(AriaRole.Tab, new() { Name = "Forms", Exact = true });
        await formsTab.ClickAsync();
        
        // Verify URL updated
        await Expect(Page).ToHaveURLAsync(new Regex("Tab=FormsTab"));
        
        // Verify forms are visible (e.g. "auto" or "autot")
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "autot", Exact = true }).First).ToBeVisibleAsync();
    }

    [Test]
    public async Task Word2WordMatchGameRendering()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + "word-2-word-match-game/en");
        
        // Wait for game to load (spinner should disappear)
        await Page.WaitForSelectorAsync(".spinner-border", new() { State = WaitForSelectorState.Hidden });
        
        // Verify heading
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Match word pairs" })).ToBeVisibleAsync();
        
        // Verify that there are cards (5 pairs + 5 links = 15 cards total roughly, but let's just check for some cards)
        var gameCards = Page.Locator(".card[role='button']");
        await Expect(gameCards).ToHaveCountAsync(10); // 5 English + 5 Estonian
    }

    [Test]
    public async Task TranslateGameRendering()
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
    public async Task ResponsiveNavMenu()
    {
        await Page.SetViewportSizeAsync(400, 800);
        await Page.GotoAsync(RootUri.AbsoluteUri);
        
        // Navbar toggler should be visible
        var toggler = Page.Locator(".navbar-toggler");
        await Expect(toggler).ToBeVisibleAsync();
        
        // Nav links should be hidden initially
        var navLinks = Page.Locator(".nav-scrollable");
        // Depending on implementation, it might be hidden or just off-screen. 
        // Let's click the toggler.
        await toggler.ClickAsync();
        
        // Now "Exercises" should be visible
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Exercises" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task FavoritesPageRendering()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + "favorites");
        
        // It should probably show a message that no favorites are found or just the heading
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Favorites" })).ToBeVisibleAsync();
    }
}
