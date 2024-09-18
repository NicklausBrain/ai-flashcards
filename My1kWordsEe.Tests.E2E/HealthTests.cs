using System.Text.RegularExpressions;

using Microsoft.Playwright;

namespace My1kWordsEe.Tests.E2E;

public class HealthTests : BlazorTest
{
    private Task _host;

    [SetUp]
    public void Setup()
    {
        // _host = Task.Run(() => Program.Main(new string[] { }));
    }

    [TearDown]
    public void TearDown()
    {
        //_host();
    }

    [Test]
    public async Task Test1()
    {
        await Page.GotoAsync(RootUri.AbsoluteUri, new() { WaitUntil = WaitUntilState.NetworkIdle });

        //await Page.GotoAsync("http://localhost:5271/");

        await Expect(Page).ToHaveTitleAsync(new Regex("Words"));

    }
}
