// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

/// <summary>
/// Class represents a book from the Open Library API.
/// </summary>
public record OpenLibraryBook(

    /// <summary>
    /// The Open Library Key of the entry.
    /// </summary>
    [property: JsonPropertyName("key")] string Key,

    /// <summary>
    /// The title of the work.
    /// </summary
    [property: JsonPropertyName("title")] string Title,

    /// <summary>
    /// The authors of the work.
    /// </summary>
    [property: JsonPropertyName("author_name")] List<string>? AuthorNames,

    /// <summary>
    /// The International Standard Book Number (ISBN) of the work.
    /// </summary>
    [property: JsonPropertyName("isbn")] List<string>? Isbn,

    /// <summary>
    /// The languages the work is availalbe in.
    /// </summary>
    [property: JsonPropertyName("language")] List<string>? Languages,

    /// <summary>
    /// The subject of the work.
    /// </summary>
    [property: JsonPropertyName("subject")] List<string>? Subjects);
