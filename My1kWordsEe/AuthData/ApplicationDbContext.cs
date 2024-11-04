using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace My1kWordsEe.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>()
                .Property(b => b.ConcurrencyStamp)
                .IsETagConcurrency();
            builder.Entity<ApplicationUser>() // ApplicationUser mean the Identity user 'ApplicationUser : IdentityUser'
                .Property(b => b.ConcurrencyStamp)
                .IsETagConcurrency();
        }
    }
}
