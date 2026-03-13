using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace BlazorApp26.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultContainer(nameof(ApplicationDbContext));
            ConfigureIdentityDiscriminators(builder);

            // Configure Cosmos DB ETag concurrency for all Identity entities
            builder.Entity<ApplicationUser>()
                .Property(u => u.ConcurrencyStamp)
                .IsETagConcurrency();

            builder.Entity<IdentityRole>()
                .Property(r => r.ConcurrencyStamp)
                .IsETagConcurrency();


            RemoveIdentityIndexes(builder);
        }

        private static void ConfigureIdentityDiscriminators(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue("ApplicationUser");

            builder.Entity<IdentityRole>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue("IdentityRole");

            builder.Entity<IdentityUserClaim<string>>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue("IdentityUserClaim");

            builder.Entity<IdentityUserLogin<string>>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue("IdentityUserLogin");

            builder.Entity<IdentityUserRole<string>>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue("IdentityUserRole");

            builder.Entity<IdentityUserToken<string>>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue("IdentityUserToken");

            builder.Entity<IdentityRoleClaim<string>>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue("IdentityRoleClaim");
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
