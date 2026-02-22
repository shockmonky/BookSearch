// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public class User
{
    /// <summary>
    /// Gets or sets db Id of the entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets name of the user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets users favorite book list.
    /// </summary>
    [JsonIgnore]
    public List<FavoriteBook> FavoriteBooks { get; set; } = [];
}
