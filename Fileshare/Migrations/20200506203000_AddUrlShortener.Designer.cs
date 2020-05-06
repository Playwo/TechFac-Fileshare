﻿// <auto-generated />
using System;
using Fileshare.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Fileshare.Migrations
{
    [DbContext(typeof(WebShareContext))]
    [Migration("20200506203000_AddUrlShortener")]
    partial class AddUrlShortener
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Fileshare.Models.LocalFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Checksum")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Checksum")
                        .IsUnique();

                    b.ToTable("LocalFiles");
                });

            modelBuilder.Entity("Fileshare.Models.PreviewOptions", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("BackgroundColor")
                        .HasColumnType("integer");

                    b.Property<int>("BoxColor")
                        .HasColumnType("integer");

                    b.Property<bool>("RedirectAgents")
                        .HasColumnType("boolean");

                    b.Property<int>("RedirectCategories")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.ToTable("PreviewOptions");
                });

            modelBuilder.Entity("Fileshare.Models.RedirectTarget", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TargetUrl")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TargetUrl")
                        .IsUnique();

                    b.ToTable("RedirectTargets");
                });

            modelBuilder.Entity("Fileshare.Models.ShortUrl", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("TargetId")
                        .HasColumnType("uuid");

                    b.Property<int>("UseCount")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TargetId");

                    b.HasIndex("UserId");

                    b.ToTable("ShortUrls");
                });

            modelBuilder.Entity("Fileshare.Models.Upload", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ContentType")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Extension")
                        .HasColumnType("text");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Uploads");
                });

            modelBuilder.Entity("Fileshare.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Balance")
                        .HasColumnType("double precision");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Fileshare.Models.PreviewOptions", b =>
                {
                    b.HasOne("Fileshare.Models.User", "User")
                        .WithOne("PreviewOptions")
                        .HasForeignKey("Fileshare.Models.PreviewOptions", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fileshare.Models.ShortUrl", b =>
                {
                    b.HasOne("Fileshare.Models.RedirectTarget", "Target")
                        .WithMany("ShortUrls")
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fileshare.Models.User", "User")
                        .WithMany("ShortUrls")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fileshare.Models.Upload", b =>
                {
                    b.HasOne("Fileshare.Models.LocalFile", "LocalFile")
                        .WithMany("Uploads")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fileshare.Models.User", "User")
                        .WithMany("Uploads")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
