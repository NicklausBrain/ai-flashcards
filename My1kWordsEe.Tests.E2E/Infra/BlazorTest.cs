using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright.NUnit;

namespace My1kWordsEe.Tests.E2E.Infra;

public abstract class BlazorTest : PageTest
{
    private IHost? _host;

    protected Uri RootUri { get; private set; } = default!;

    /// <summary>
    /// The service provider of the running test host. Available after setup.
    /// Use it to resolve services (e.g. for data cleanup) inside a test or teardown.
    /// </summary>
    protected IServiceProvider Services => _host!.Services;

    /// <summary>
    /// Extra command-line arguments a derived fixture wants to pass to the host
    /// (e.g. feature flags). Defaults to none.
    /// </summary>
    protected virtual IEnumerable<string> ExtraHostArgs => Array.Empty<string>();

    /// <summary>
    /// Hook invoked at the very start of teardown, before the host is stopped and
    /// disposed. Override to perform cleanup that needs the host services (e.g.
    /// deleting test data). The host is still running when this is called.
    /// </summary>
    protected virtual Task OnTearDownAsync() => Task.CompletedTask;

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
        }.Concat(ExtraHostArgs).ToArray();
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
            try
            {
                await OnTearDownAsync();
            }
            finally
            {
                await _host.StopAsync();
                _host.Dispose();
            }
        }
    }
}