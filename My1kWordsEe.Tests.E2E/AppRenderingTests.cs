using System.Text.RegularExpressions;

using Microsoft.Playwright;

using My1kWordsEe.Tests.E2E.Infra;

namespace My1kWordsEe.Tests.E2E;

public class AppRenderingTests : BlazorTest
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
}