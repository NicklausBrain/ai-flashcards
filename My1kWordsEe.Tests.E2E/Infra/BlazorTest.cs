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
        _host = Program.BuildWebHost(new string[] { });

        await _host.StartAsync();

        var addrs = _host.Services.GetRequiredService<IServer>().Features
            .GetRequiredFeature<IServerAddressesFeature>()
            .Addresses;

        RootUri = new(_host.Services.GetRequiredService<IServer>().Features
            .GetRequiredFeature<IServerAddressesFeature>()
            .Addresses.Single());
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