using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace My1kWordsEe.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            RemoveIdentityIndexes(builder);
            builder.Entity<IdentityRole>()
                .Property(b => b.ConcurrencyStamp)
                .IsETagConcurrency();
            builder.Entity<ApplicationUser>() // ApplicationUser mean the Identity user 'ApplicationUser : IdentityUser'
                .Property(b => b.ConcurrencyStamp)
                .IsETagConcurrency();
        }

        private static void RemoveIdentityIndexes(ModelBuilder builder)
        {
            var roleEntity = builder.Entity<IdentityRole>();
            RemoveIndex(roleEntity, nameof(IdentityRole.NormalizedName));

            var userEntity = builder.Entity<ApplicationUser>();
            RemoveIndex(userEntity, nameof(IdentityUser.NormalizedUserName));
            RemoveIndex(userEntity, nameof(IdentityUser.NormalizedEmail));
        }

        private static void RemoveIndex<TEntity>(EntityTypeBuilder<TEntity> entity, string propertyName)
            where TEntity : class
        {
            var property = entity.Property(propertyName).Metadata;
            var index = entity.Metadata.FindIndex(new[] { property });
            if (index != null)
            {
                entity.Metadata.RemoveIndex(index);
            }
        }
    }
}