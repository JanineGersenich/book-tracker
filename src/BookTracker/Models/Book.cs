using System;

namespace BookTracker.Models;

/// <summary>
/// Repräsentiert ein Buch in der Bibliothek.
/// Nutzt eine künstliche Auto-Increment-Id als Primary Key statt der ISBN,
/// da nicht jedes Buch eine ISBN besitzt (z. B. ältere Ausgaben) und
/// verschiedene Auflagen desselben Werks unterschiedliche ISBNs haben können.
/// </summary>
public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional, da nicht jedes Buch eine ISBN hat. Eindeutig, falls vorhanden
    /// (siehe Unique-Index in BookTrackerContext.OnModelCreating).
    /// </summary>
    public string? Isbn { get; set; }

    public string Genre { get; set; } = string.Empty;

    /// <summary>Bewertung 1-5 Sterne, null solange noch nicht bewertet.</summary>
    public int? Rating { get; set; }

    public ReadingStatus Status { get; set; } = ReadingStatus.ToRead;

    public int? PageCount { get; set; }

    public DateTime? DateStarted { get; set; }

    public DateTime? DateFinished { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int AuthorId { get; set; }

    public Author? Author { get; set; }
}
