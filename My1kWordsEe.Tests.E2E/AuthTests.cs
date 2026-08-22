using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

using My1kWordsEe.Data;
using My1kWordsEe.Tests.E2E.Infra;

namespace My1kWordsEe.Tests.E2E;

[TestFixture]
public class AuthTests : BlazorTest
{
    private const string Password = "Test-Password-123!";

    private readonly List<string> _createdEmails = new();

    // Enable the registration UI, which is otherwise disabled by default.
    protected override IEnumerable<string> ExtraHostArgs => new[] { "--IsRegistrationEnabled=true" };

    private string NewUniqueEmail()
    {
        var email = $"e2e-{Guid.NewGuid():N}@example.com";
        _createdEmails.Add(email);
        return email;
    }

    private async Task RegisterNewUserAsync(string email, string password)
    {
        await Page.GotoAsync(RootUri.AbsoluteUri + "Account/Register", new() { WaitUntil = WaitUntilState.DOMContentLoaded });

        await Page.GetByPlaceholder("name@example.com").FillAsync(email);
        // Two password fields share the "password" placeholder: Password and Confirm Password.
        var passwordFields = Page.GetByPlaceholder("password");
        await passwordFields.First.FillAsync(password);
        await passwordFields.Nth(1).FillAsync(password);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    private async Task LogoutAsync()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Logout" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Test]
    public async Task Register_SignsInNewUser()
    {
        var email = NewUniqueEmail();

        await RegisterNewUserAsync(email, Password);

        // After registration the user is signed in (no email confirmation required),
        // so the NavMenu should show the Logout button.
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Logout" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Logout_SignsOutUser()
    {
        var email = NewUniqueEmail();
        await RegisterNewUserAsync(email, Password);

        await LogoutAsync();

        // Anonymous nav should show Login and Register links again.
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Login" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Register" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Login_WithValidCredentials_SignsInUser()
    {
        var email = NewUniqueEmail();
        await RegisterNewUserAsync(email, Password);
        await LogoutAsync();

        await Page.GotoAsync(RootUri.AbsoluteUri + "Account/Login", new() { WaitUntil = WaitUntilState.DOMContentLoaded });
        await Page.GetByPlaceholder("name@example.com").FillAsync(email);
        await Page.GetByPlaceholder("password").FillAsync(Password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Logout" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Login_WithInvalidPassword_ShowsError()
    {
        var email = NewUniqueEmail();
        await RegisterNewUserAsync(email, Password);
        await LogoutAsync();

        await Page.GotoAsync(RootUri.AbsoluteUri + "Account/Login", new() { WaitUntil = WaitUntilState.DOMContentLoaded });
        await Page.GetByPlaceholder("name@example.com").FillAsync(email);
        await Page.GetByPlaceholder("password").FillAsync("Wrong-Password-999!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.GetByText("Error: Invalid login attempt.")).ToBeVisibleAsync();
    }

    protected override async Task OnTearDownAsync()
    {
        // Remove any accounts created during the test so the Auth store stays clean.
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var email in _createdEmails)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                await userManager.DeleteAsync(user);
            }
        }

        _createdEmails.Clear();
    }
}
