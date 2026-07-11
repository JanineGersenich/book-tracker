using System;
using System.IO;
using BookTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Data;

public class BookTrackerContext : DbContext
{
    private readonly string _dbPath;

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();

    public BookTrackerContext()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(localAppData, "BookTracker");
        Directory.CreateDirectory(appFolder);
        _dbPath = Path.Combine(appFolder, "booktracker.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ISBN ist optional, aber eindeutig, falls vorhanden.
        // SQLite erlaubt mehrere NULL-Werte trotz Unique-Index - passt genau
        // zu Büchern ohne ISBN.
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Isbn)
            .IsUnique();

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Author>()
            .HasIndex(a => a.Name)
            .IsUnique();
    }
}
