using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;

namespace Fileshare.Models
{
    public class WebShareContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<PreviewOptions> PreviewOptions { get; set; }
        public DbSet<LocalFile> LocalFiles { get; set; }
        public DbSet<Upload> Uploads { get; set; }

        public DbSet<ShortUrl> ShortUrls {get; set;}
        public DbSet<RedirectTarget> RedirectTargets { get; set; }

        public WebShareContext(DbContextOptions options)
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

                b.HasMany(x => x.ShortUrls)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

                b.ToTable("Users");
            });

            modelBuilder.Entity<PreviewOptions>(b =>
            {
                b.Property(x => x.UserId);
                b.HasKey(x => x.UserId);

                b.Property(x => x.RedirectAgents);

                b.Property(x => x.RedirectCategories);

                b.Property(x => x.BackgroundColor)
                .HasConversion(
                    x => x.ToArgb(),         //Store
                    x => Color.FromArgb(x)); //Load

                b.Property(x => x.BoxColor)
                .HasConversion(
                    x => x.ToArgb(),         //Store
                    x => Color.FromArgb(x)); //Load

                b.ToTable("PreviewOptions");
            });

            modelBuilder.Entity<LocalFile>(b =>
            {
                b.Property(x => x.Id);
                b.HasKey(x => x.Id);

                b.Property(x => x.Checksum);
                b.HasIndex(x => x.Checksum)
                .IsUnique();

                b.Property(x => x.CreatedAt);

                b.HasMany(x => x.Uploads)
                .WithOne(x => x.LocalFile)
                .HasForeignKey(x => x.FileId);

                b.ToTable("LocalFiles");
            });

            modelBuilder.Entity<Upload>(b =>
            {
                b.Property(x => x.Id);
                b.HasKey(x => x.Id);

                b.Property(x => x.UserId);
                b.HasIndex(x => x.UserId);

                b.Property(x => x.Name);
                b.HasIndex(x => x.Name)
                .IsUnique();

                b.Ignore(x => x.Filename);

                b.Property(x => x.ContentType);

                b.Property(x => x.CreatedAt);

                b.ToTable("Uploads");
            });

            modelBuilder.Entity<RedirectTarget>(b =>
            {
                b.Property(x => x.Id);
                b.HasKey(x => x.Id);

                b.Property(x => x.TargetUrl);
                b.HasIndex(x => x.TargetUrl)
                .IsUnique();

                b.Property(x => x.CreatedAt);

                b.HasMany(x => x.ShortUrls)
                .WithOne(x => x.Target)
                .HasForeignKey(x => x.TargetId);

                b.ToTable("RedirectTargets");
            });

            modelBuilder.Entity<ShortUrl>(b =>
            {
                b.Property(x => x.Id);
                b.HasKey(x => x.Id);

                b.Property(x => x.UserId);

                b.Property(x => x.TargetId);

                b.Property(x => x.CreatedAt);
                b.Property(x => x.Name);
                b.Property(x => x.UseCount);

                b.ToTable("ShortUrls");
            });
        }
    }
}
