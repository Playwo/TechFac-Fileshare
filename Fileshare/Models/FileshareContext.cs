using Microsoft.EntityFrameworkCore;

namespace Fileshare.Models
{
    public class FileshareContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<PreviewOptions> PreviewOptions { get; set; }
        public DbSet<Upload> Uploads { get; set; }

        public FileshareContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b =>
            {
                b.Property(x => x.Id);
                b.HasKey(x => x.Id);

                b.Property(x => x.Username);
                b.HasIndex(x => x.Username)
                .IsUnique();

                b.Property(x => x.PasswordHash);

                b.Property(x => x.Token);
                b.HasIndex(x => x.Token)
                .IsUnique();

                b.Property(x => x.CreatedAt);

                b.Property(x => x.Balance);

                b.HasMany(x => x.Uploads)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

                b.HasOne(x => x.PreviewOptions)
                .WithOne(x => x.User)
                .HasForeignKey<PreviewOptions>(x => x.UserId);
            });

            modelBuilder.Entity<PreviewOptions>(b =>
            {
                b.Property(x => x.UserId);
                b.HasKey(x => x.UserId);

                b.Property(x => x.RedirectAgents);

                b.Property(x => x.RedirectCategories);
            });

            modelBuilder.Entity<Upload>(b =>
            {
                b.Property(x => x.Id);
                b.HasKey(x => x.Id);

                b.Property(x => x.UserId);
                b.HasIndex(x => x.UserId);

                b.Property(x => x.Filename);
                b.HasIndex(x => x.Filename)
                .IsUnique();

                b.Property(x => x.ContentType);

                b.Property(x => x.CreatedAt);
            });
        }
    }
}
