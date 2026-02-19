// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

/// <summary>
/// Class represents a book from the Open Library API.
/// </summary>
public class FavoriteBook
{
    /// <summary>
    /// Gets or sets db Id of the entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets open Library Key of the entry.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets title of the work.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets authors of the work.
    /// </summary>
    public string? Authors { get; set; }

    /// <summary>
    /// Gets or sets uRL for the cover of the work.
    /// </summary>
    public string? CoverUrl { get; set; }

    /// <summary>
    /// Gets or sets international Standard Book Number (ISBN) of the work.
    /// </summary>
    public string? PrimaryIsbn { get; set; }

    /// <summary>
    /// Gets or sets dateTime that the favorite was added.
    /// </summary>
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
