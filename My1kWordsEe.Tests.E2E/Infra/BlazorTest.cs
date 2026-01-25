using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright.NUnit;

namespace My1kWordsEe.Tests.E2E.Infra;

public class BlazorTest : PageTest
{
    private IHost? _host;

    protected Uri RootUri { get; private set; } = default!;

    [SetUp]
    public async Task SetUpWebApplication()
    {
        var executionDir = Path.GetDirectoryName(typeof(BlazorTest).Assembly.Location)!;
        var webRootPath = Path.Combine(executionDir, "../../../../My1kWordsEe/wwwroot");
        var args = new[] 
        { 
            $"--webroot={Path.GetFullPath(webRootPath)}",
            "--environment=Development",
            "--applicationName=My1kWordsEe"
        };
        _host = Program.BuildWebHost(args);

        await _host.StartAsync();

        var addrs = _host.Services.GetRequiredService<IServer>().Features
            .GetRequiredFeature<IServerAddressesFeature>()
            .Addresses;

        RootUri = new(addrs.Single());
    }

    [TearDown]
    public async Task TearDownWebApplication()
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}