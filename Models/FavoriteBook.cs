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
    /// Gets or sets UserId that favorited this book.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets open Library Key of the entry.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets dateTime that the favorite was added.
    /// </summary>
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets navigation property to link a book to a user that has favorited it.
    /// </summary>
    [JsonIgnore]
    public User User { get; set; } = null!;
}
