using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using My1kWordsEe.Data;

namespace My1kWordsEe.Components.Account
{
    internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
    {
        public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            }

            return user;
        }

        public async Task<ApplicationUser> GetRequiredUserAsync(ClaimsPrincipal user)
        {
            var appUser = await userManager.GetUserAsync(user);

            if (appUser is null)
            {
                redirectManager.RedirectTo("Account/Login");
            }

            return appUser;
        }

        public async Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal user)
        {
            var appUser = await userManager.GetUserAsync(user);

            return appUser;
        }
    }
}