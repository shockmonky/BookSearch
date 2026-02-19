// Take home project for Matthew Maffett

namespace BookSearchApi.Models;

public class User
{
    /// <summary>
    /// Gets or sets db Id of the entry.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets name of the user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets users favorite book list.
    /// </summary>
    public List<FavoriteBook> FavoriteBooks { get; set; } = [];
}
