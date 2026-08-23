using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;

namespace My1kWordsEe.Tests.Unit.Common
{
    public sealed class TestAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthenticationState authenticationState;

        public TestAuthenticationStateProvider(ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);
            authenticationState = new AuthenticationState(user);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(authenticationState);

        public static TestAuthenticationStateProvider CreateAuthenticated(string userId) =>
            new(CreateUser(userId, isAuthenticated: true));

        public static TestAuthenticationStateProvider CreateUnauthenticated() =>
            new(CreateUser(userId: string.Empty, isAuthenticated: false));

        private static ClaimsPrincipal CreateUser(string userId, bool isAuthenticated)
        {
            var claims = new List<Claim>();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            }

            var identity = new ClaimsIdentity(claims, isAuthenticated ? "TestAuth" : string.Empty);
            return new ClaimsPrincipal(identity);
        }
    }
}
