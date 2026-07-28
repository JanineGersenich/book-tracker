using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookTracker.Data;
using BookTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Services;

/// <summary>
/// Kapselt sämtliche Datenbankzugriffe. Jede Methode öffnet einen eigenen,
/// kurzlebigen DbContext (empfohlenes Pattern für Desktop-Apps ohne DI-Container).
/// </summary>
public class BookRepository
{
    private static BookTrackerContext CreateContext() => new();

    public async Task<List<Book>> GetAllBooksAsync()
    {
        using var context = CreateContext();
        return await context.Books
            .Include(b => b.Author)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<string>> GetAllAuthorNamesAsync()
    {
        using var context = CreateContext();
        return await context.Authors
            .OrderBy(a => a.Name)
            .Select(a => a.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Legt das Buch an oder aktualisiert es. Der Autor wird per Name gesucht
    /// oder neu angelegt (einfacher "Find-or-Create"-Ansatz für dieses Portfolio-Projekt).
    /// </summary>
    public async Task SaveBookAsync(Book book, string authorName)
    {
        using var context = CreateContext();

        var author = await context.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null)
        {
            author = new Author { Name = authorName };
            context.Authors.Add(author);
        }

        if (book.Id == 0)
        {
            book.Author = author;
            book.CreatedAt = DateTime.UtcNow;
            context.Books.Add(book);
        }
        else
        {
            var existing = await context.Books.FirstAsync(b => b.Id == book.Id);
            existing.Title = book.Title;
            existing.Isbn = book.Isbn;
            existing.Genre = book.Genre;
            existing.Rating = book.Rating;
            existing.Status = book.Status;
            existing.PageCount = book.PageCount;
            existing.DateStarted = book.DateStarted;
            existing.DateFinished = book.DateFinished;
            existing.Notes = book.Notes;
            existing.AuthorId = author.Id;
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        using var context = CreateContext();
        var book = await context.Books.FindAsync(id);
        if (book != null)
        {
            context.Books.Remove(book);
            await context.SaveChangesAsync();
        }
    }

    // ---- Analyse-Queries ----

    public async Task<Dictionary<string, int>> GetBookCountByGenreAsync()
    {
        using var context = CreateContext();
        return await context.Books
            .Where(b => b.Status == ReadingStatus.Finished && b.Genre != "")
            .GroupBy(b => b.Genre)
            .Select(g => new { Genre = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToDictionaryAsync(x => x.Genre, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetTopAuthorsAsync(int top = 5)
    {
        using var context = CreateContext();
        var grouped = await context.Books
            .Where(b => b.Status == ReadingStatus.Finished)
            .Include(b => b.Author)
            .GroupBy(b => b.Author!.Name)
            .Select(g => new { Author = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(top)
            .ToListAsync();

        return grouped.ToDictionary(x => x.Author, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetBooksPerYearAsync()
    {
        using var context = CreateContext();
        var finished = await context.Books
            .Where(b => b.Status == ReadingStatus.Finished && b.DateFinished != null)
            .ToListAsync();

        return finished
            .GroupBy(b => b.DateFinished!.Value.Year)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());
    }

    public async Task<double> GetAverageRatingAsync()
    {
        using var context = CreateContext();
        var ratings = await context.Books
            .Where(b => b.Rating != null)
            .Select(b => b.Rating!.Value)
            .ToListAsync();

        return ratings.Count > 0 ? ratings.Average() : 0;
    }

    public async Task<int> GetTotalPagesReadAsync()
    {
        using var context = CreateContext();
        return await context.Books
            .Where(b => b.Status == ReadingStatus.Finished)
            .SumAsync(b => b.PageCount ?? 0);
    }
}
